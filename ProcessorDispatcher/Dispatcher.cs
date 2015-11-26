using System;
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

        private readonly object executionLock = new object();
        /// <summary>Mapping of processor ids to last execution times. This should only be accessed if a lock to <see cref="executionLock"/> was acquired.</summary>
        private readonly Dictionary<Guid, DateTime> lastExecutions = new Dictionary<Guid, DateTime>();
        /// <summary></summary>
        private bool running = false;

        public Dispatcher(IProcessorSimulator simulator, DispatcherItemFactory itemFactory, UnityContainer container)
        {
            this.simulator = simulator;
            this.container = container;
            this.itemFactory = itemFactory;
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
            } while (item != null && !processors.TryUpdate(id, item, update(item)));
            return item != null;
        }
        public bool Remove(Guid id)
        {
            IDispatcherItem item;
            return processors.TryRemove(id, out item);
        }

        public void Step(Guid id, SimulationStepSize stepSize)
        {
            stepRequests.AddOrUpdate(id, ImmutableQueue.Create(stepSize), (key, list) => list.Enqueue(stepSize));
        }

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

        /// <summary>Execute steps for every processor, which has a pending request or which is running automatically.</summary>
        private void Execute()
        {
            lock (executionLock)
            {
                if (cancel.IsCancellationRequested) { return; }
                var executions = new Dictionary<Guid, SimulationStepSize>();
                AddStepRequests(executions);
                AddRunningProcessors(executions);
                foreach(var execution in executions)
                {
                    IDispatcherItem item;
                    if(!processors.TryGetValue(execution.Key, out item)) { continue; }
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
                    lastExecutions[item.Guid] = DateTime.Now;
                }
            }
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
