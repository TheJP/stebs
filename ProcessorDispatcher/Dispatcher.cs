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
        private readonly ConcurrentDictionary<Guid, ImmutableList<SimulationStepSize>> stepRequests = new ConcurrentDictionary<Guid, ImmutableList<SimulationStepSize>>();

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
        public bool Update(IDispatcherItem updated, IDispatcherItem comparison) => processors.TryUpdate(updated.Guid, updated, comparison);
        public bool Remove(Guid id)
        {
            IDispatcherItem item;
            return processors.TryRemove(id, out item);
        }

        public void Step(Guid id, SimulationStepSize stepSize)
        {
            stepRequests.AddOrUpdate(id, ImmutableList.Create(stepSize), (key, list) => list.Add(stepSize));
        }

        /// <summary>Collect execution stebs, which were triggered manually on the client side.</summary>
        /// <param name="executions"></param>
        private void AddStepRequests(Dictionary<Guid, SimulationStepSize> executions)
        {
            foreach (var request in stepRequests)
            {
                //Remove first request from the current processor step queue
                var stepSizeList = request.Value;
                while (!stepRequests.TryUpdate(request.Key, stepSizeList.RemoveAt(0), stepSizeList))
                {
                    if (!stepRequests.TryGetValue(request.Key, out stepSizeList)) { break; }
                }
                //Add the removed request to the executions
                if (stepSizeList != null && stepSizeList.Count > 0)
                {
                    SimulationStepSize stepSize = stepSizeList[0];
                    executions.Add(request.Key, stepSize);
                }
                //Remove the step queue if it is empty
                if (stepSizeList != null && stepSizeList.Count <= 1)
                {
                    stepRequests.TryRemove(request.Key, out stepSizeList);
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
<<<<<<< HEAD
=======
            lock (executionLock)
            {
                running = true;
                tokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(Execute, cancel);
            }
        }

        public void Stop()
        {
            lock (executionLock)
            {
                tokenSource.Cancel();
                running = false;
            }
>>>>>>> 5bc0c5e158dd5b656bee37ecd99c9e2658b67215
        }
    }
}
