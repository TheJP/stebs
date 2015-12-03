using ProcessorSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    public class DispatcherItem : IDispatcherItem
    {
        public static DispatcherItemFactory Factory { get; } = (guid, processor, running, runDelay, stepSize) => new DispatcherItem(guid, processor, running, runDelay, stepSize);
        public Guid Guid { get; }
        public IProcessor Processor { get; }
        public bool Running { get; }
        public TimeSpan RunDelay { get; }
        public SimulationStepSize StepSize { get; }

        public DispatcherItem(Guid guid, IProcessor processor, bool running, TimeSpan runDelay, SimulationStepSize stepSize)
        {
            this.Guid = guid;
            this.Processor = processor;
            this.Running = running;
            this.RunDelay = runDelay;
            this.StepSize = stepSize;
        }
        public IDispatcherItem SetRunning(bool value)
        {
            if(this.Running == value) { return this; }
            return new DispatcherItem(Guid, Processor, value, RunDelay, StepSize);
        }
        public IDispatcherItem SetRunDelay(TimeSpan runDelay)
        {
            if(this.RunDelay == runDelay) { return this; }
            return new DispatcherItem(Guid, Processor, Running, runDelay, StepSize);
        }
        public IDispatcherItem SetStepSize(SimulationStepSize stepSize)
        {
            if (this.StepSize == stepSize) { return this; }
            return new DispatcherItem(Guid, Processor, Running, RunDelay, stepSize);
        }
    }
}
