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
        public static DispatcherItemFactory Factory { get; } = (guid, processor, running, runDelay) => new DispatcherItem(guid, processor, running, runDelay);
        public Guid Guid { get; }
        public IProcessor Processor { get; }
        public bool Running { get; }
        public TimeSpan RunDelay { get; }
        public DispatcherItem(Guid guid, IProcessor processor, bool running, TimeSpan runDelay)
        {
            this.Guid = guid;
            this.Processor = processor;
            this.Running = running;
            this.RunDelay = runDelay;
        }
        public IDispatcherItem SetRunning(bool value)
        {
            return new DispatcherItem(Guid, Processor, value, RunDelay);
        }
        public IDispatcherItem SetRunDelay(TimeSpan runDelay)
        {
            return new DispatcherItem(Guid, Processor, Running, runDelay);
        }
    }
}
