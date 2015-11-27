using ProcessorDispatcher;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using ProcessorSimulation;

namespace Stebs5
{
    public class ProcessorManager : IProcessorManager
    {
        private IDispatcher Dispatcher { get; }
        private readonly ConcurrentDictionary<string, IDispatcherItem> processors = new ConcurrentDictionary<string, IDispatcherItem>();

        public ProcessorManager(IDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            this.Dispatcher.FinishedStep += FinishedStep;
        }

        /// <summary>Called, when a simulation step was processed.</summary>
        /// <param name="item">Processor which was simulated</param>
        /// <param name="stepSize">Simulated step size.</param>
        /// <param name="ramChanges">Changes done to the ram during the simulation step.</param>
        /// <param name="registerChanges">Changes done to the registers during the simulation step.</param>
        private void FinishedStep(IDispatcherItem item, SimulationStepSize stepSize, IReadOnlyDictionary<byte, byte> ramChanges, IReadOnlyDictionary<Registers, IRegister> registerChanges)
        {
            throw new NotImplementedException();
        }

        public void CreateProcessor(string clientId)
        {
            //Remove processor, if it exists for given client id
            //This assures, that a new client does not get an existing processor
            RemoveProcessor(clientId);
            //Add new processor for given client id
            AssureProcessorExists(clientId);
        }

        public void AssureProcessorExists(string clientId) =>
            //Create new processor if none exists; Use existing otherwise
            processors.AddOrUpdate(clientId, id => Dispatcher.CreateProcessor(), (id, processor) => processor);

        public void RemoveProcessor(string clientId)
        {
            IDispatcherItem processor;
            if (processors.TryRemove(clientId, out processor))
            {
                Dispatcher.Remove(processor.Guid);
            }
        }

        public void ChangeRamContent(string clientId, int[] newContent)
        {
            IDispatcherItem item;
            if(processors.TryGetValue(clientId, out item))
            {
                using (var session = item.Processor.CreateSession())
                {
                    session.RamSession.Set(newContent.Select(ram => (byte)ram).ToArray());
                }
            }
        }

        private void Update(string clientId, Func<IDispatcherItem, IDispatcherItem> update)
        {
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                Dispatcher.Update(item.Guid, update);
            }
        }

        public void Run(string clientId) => Update(clientId, item => item.SetRunning(true));
        public void Pause(string clientId) => Update(clientId, item => item.SetRunning(false));
        public void ChangeSetpSize(string clientId, SimulationStepSize stepSize) => Update(clientId, item => item.SetStepSize(stepSize));
        public void ChangeRunDelay(string clientId, TimeSpan runDelay) => Update(clientId, item => item.SetRunDelay(runDelay));

        public void Step(string clientId, SimulationStepSize stepSize)
        {
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                Dispatcher.Step(item.Guid, stepSize);
            }
        }
    }
}
