﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ProcessorSimulation;
using Microsoft.Practices.Unity;
using System.Collections.Immutable;
using System.Threading;

namespace ProcessorDispatcher
{
    public class Dispatcher : IDispatcher
    {
        private CancellationTokenSource tokenSource;
        private CancellationToken cancel => tokenSource.Token;

        private readonly IProcessorSimulator simulator;
        private readonly UnityContainer container;
        private readonly DispatcherItemFactory itemFactory;
        private readonly ConcurrentDictionary<Guid, IDispatcherItem> processors = new ConcurrentDictionary<Guid, IDispatcherItem>();
        private readonly ConcurrentDictionary<Guid, ImmutableQueue<SimulationStepSize>> stepRequests = new ConcurrentDictionary<Guid, ImmutableQueue<SimulationStepSize>>();
        private readonly ConcurrentBag<ResetRequest> resetRequests = new ConcurrentBag<ResetRequest>();

        private enum ResetType { Soft, Hard }
        private struct ResetRequest
        {
            public Guid Guid { get; set; }
            public ResetType Type { get; set; }
        }

        private readonly object executionLock = new object();
        /// <summary>Mapping of processor ids to last execution times. This should only be accessed if a lock to <see cref="executionLock"/> was acquired.</summary>
        private readonly Dictionary<Guid, DateTime> lastExecutions = new Dictionary<Guid, DateTime>();
        /// <summary>Running flag, which determines, if the dispatcher is running. Used in the Start and Stop method and should only be accessed if a lock to <see cref="executionLock"/> was acquired.</summary>
        private bool running = false;
        /// <summary>Collector, which is used to get processor and ram changes. Should only be accessed if a lock to <see cref="executionLock"/> was acquired.</summary>
        private IChangesCollector collector;

        #region Events
        //Because processor events are accessed by multiple threads custom locking has to be implemented
        //to guarantee thread safety. (See also the delegate chapter of 'C# in depth')
        private object eventLock = new object();

        private Action<IDispatcherItem, SimulationStepSize, IReadOnlyDictionary<byte, byte>, IReadOnlyDictionary<Registers, IRegister>> finishedStep;
        public event Action<IDispatcherItem, SimulationStepSize, IReadOnlyDictionary<byte, byte>, IReadOnlyDictionary<Registers, IRegister>> FinishedStep
        {
            add
            {
                lock (eventLock) { finishedStep += value; }
            }
            remove
            {
                lock (eventLock) { finishedStep -= value; }
            }
        }

        private Action<IDispatcherItem, StateChange> stateChanged;
        public event Action<IDispatcherItem, StateChange> StateChanged
        {
            add
            {
                lock (eventLock) { stateChanged += value; }
            }
            remove
            {
                lock (eventLock) { stateChanged -= value; }
            }
        }

        /// <summary>Notifies, simulation step finished.</summary>
        /// <param name="item">Processor which was simulated</param>
        /// <param name="stepSize">Simulated step size.</param>
        /// <param name="ramChanges">Changes done to the ram during the simulation step.</param>
        /// <param name="registerChanges">Changes done to the registers during the simulation step.</param>
        private void NotifyFinishedStep(IDispatcherItem item, SimulationStepSize stepSize, IReadOnlyDictionary<byte, byte> ramChanges, IReadOnlyDictionary<Registers, IRegister> registerChanges)
        {
            Action<IDispatcherItem, SimulationStepSize, IReadOnlyDictionary<byte, byte>, IReadOnlyDictionary<Registers, IRegister>> handler;
            lock (eventLock)
            {
                handler = finishedStep;
            }
            if (handler != null)
            {
                //Call handler outside of the lock, so called handle methods will not caue a deadlock.
                //This is safe because delegates are immutable.
                handler(item, stepSize, ramChanges, registerChanges);
            }
        }

        /// <summary>Notifies, when a reset or halt request finished execution.</summary>
        /// <param name="item">Processor, which changed.</param>
        private void NotifyStateChanged(IDispatcherItem item, StateChange stateChange)
        {
            Action<IDispatcherItem, StateChange> handler;
            lock (eventLock)
            {
                handler = stateChanged;
            }
            if (handler != null)
            {
                handler(item, stateChange);
            }
        }
        #endregion

        public Dispatcher(IProcessorSimulator simulator, DispatcherItemFactory itemFactory, IChangesCollector collector, UnityContainer container)
        {
            this.simulator = simulator;
            this.container = container;
            this.itemFactory = itemFactory;
            this.collector = collector;
        }

        public IDispatcherItem CreateProcessor(bool running, TimeSpan runDelay, SimulationStepSize stepSize)
        {
            IDispatcherItem item;
            do
            {
                item = itemFactory(Guid.NewGuid(), container.Resolve<IProcessor>(), running, runDelay, stepSize);
            } while (!processors.TryAdd(item.Guid, item));
            return item;
        }

        public IDispatcherItem this[Guid id] => processors[id];
        public bool ContainsGuid(Guid id) => processors.ContainsKey(id);
        public bool Update(Guid id, Func<IDispatcherItem, IDispatcherItem> update)
        {
            IDispatcherItem item;
            do
            {
                var success = processors.TryGetValue(id, out item);
                if (!success) { item = null; }
            } while (item != null && !processors.TryUpdate(id, update(item), item));
            return item != null;
        }
        public bool Remove(Guid id)
        {
            IDispatcherItem item;
            return processors.TryRemove(id, out item);
        }

        public void Step(Guid id, SimulationStepSize stepSize) =>
            stepRequests.AddOrUpdate(id, ImmutableQueue.Create(stepSize), (key, list) => list.Enqueue(stepSize));

        public void SoftReset(Guid id) => resetRequests.Add(new ResetRequest() { Guid = id, Type = ResetType.Soft });

        public void HardReset(Guid id) => resetRequests.Add(new ResetRequest() { Guid = id, Type = ResetType.Hard });

        /// <summary>Collect execution stebs, which were triggered manually on the client side.</summary>
        /// <param name="executions"></param>
        private void AddStepRequests(Dictionary<Guid, SimulationStepSize> executions)
        {
            foreach (var request in stepRequests)
            {
                //Remove first request from the current processor step queue
                var stepSizeQueue = request.Value;
                SimulationStepSize stepSize;
                while (!stepRequests.TryUpdate(request.Key, stepSizeQueue.Dequeue(out stepSize), stepSizeQueue))
                {
                    if (!stepRequests.TryGetValue(request.Key, out stepSizeQueue)) { break; }
                }
                //Add the removed request to the executions
                if (stepSizeQueue != null){ executions.Add(request.Key, stepSize); }
                //Remove the step queue if it is empty
                if (stepSizeQueue != null && stepSizeQueue.Dequeue(out stepSize).IsEmpty)
                {
                    stepRequests.TryRemove(request.Key, out stepSizeQueue);
                }
            }
        }


        /// <summary>Collect running processors, which need an automatic execution step.</summary>
        /// <param name="executions"></param>
        private void AddRunningProcessors(Dictionary<Guid, SimulationStepSize> executions)
        {
            //Add each processor, which was not added, is running
            foreach (var item in processors.Values
                .Where(item => item.Running)
                //Don't add processors, which have run below the minimum delay
                .Where(item => !lastExecutions.ContainsKey(item.Guid) || lastExecutions[item.Guid] + item.RunDelay <= DateTime.Now))
            {
                if (!executions.ContainsKey(item.Guid))
                {
                    executions.Add(item.Guid, item.StepSize);
                }
            }
        }

        /// <summary>Executes requested resets.</summary>
        /// <remarks>Should only be called with an aquired lock on <see cref="executionLock"/>.</remarks>
        private void ExecuteResets()
        {
            if (resetRequests.IsEmpty) { return; }
            var requests = new HashSet<ResetRequest>();
            ResetRequest reset;
            while (!resetRequests.IsEmpty && resetRequests.TryTake(out reset)) { requests.Add(reset); }
            foreach(var request in requests)
            {
                IDispatcherItem item;
                if(processors.TryGetValue(request.Guid, out item))
                {
                    if (request.Type == ResetType.Soft)
                    {
                        simulator.SoftReset(item.Processor);
                        NotifyStateChanged(item, StateChange.SoftReset);
                    }
                    else
                    {
                        simulator.HardReset(item.Processor);
                        NotifyStateChanged(item, StateChange.HardReset);
                    }
                }
            }
        }

        /// <summary>Execute steps for every processor, which has a pending request or which is running automatically.</summary>
        private void Execute()
        {
            lock (executionLock)
            {
                if (cancel.IsCancellationRequested) { return; }
                ExecuteResets();
                var executions = new Dictionary<Guid, SimulationStepSize>();
                AddStepRequests(executions);
                AddRunningProcessors(executions);
                foreach(var execution in executions)
                {
                    IDispatcherItem item;
                    if(!processors.TryGetValue(execution.Key, out item)) { continue; }
                    collector.BindTo(item.Processor);
                    try
                    {
                        switch (execution.Value)
                        {
                            case SimulationStepSize.Instruction:
                                simulator.ExecuteInstructionStep(item.Processor);
                                break;
                            case SimulationStepSize.Macro:
                                simulator.ExecuteMacroStep(item.Processor);
                                break;
                            case SimulationStepSize.Micro:
                                simulator.ExecuteMicroStep(item.Processor);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        //TODO: proper error handling
                        //e.g. Halt on "div by 0" or not implemented exceptions
                    }
                    collector.Unbind();
                    NotifyFinishedStep(item, execution.Value, collector.RamChanges, collector.RegisterChanges);
                    if (collector.IsHalted)
                    {
                        Update(item.Guid, i => i.SetRunning(false));
                        NotifyStateChanged(item, StateChange.Halt);
                    }
                    lastExecutions[item.Guid] = DateTime.Now;
                }
            }
#if DEBUG
            //TODO: Potential performance improvement
            Thread.Sleep(10);
#endif
            Task.Factory.StartNew(Execute, cancel);
        }

        public void Start()
        {
            lock (executionLock)
            {
                if (!running)
                {
                    running = true;
                    tokenSource = new CancellationTokenSource();
                    Task.Factory.StartNew(Execute, cancel);
                }
            }
        }

        public void Stop()
        {
            lock (executionLock)
            {
                if (running)
                {
                    tokenSource.Cancel();
                    running = false;
                }
            }
        }
    }
}
