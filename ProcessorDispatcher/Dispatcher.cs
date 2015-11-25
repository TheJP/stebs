using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ProcessorSimulation;

namespace ProcessorDispatcher
{
    public class Dispatcher : IDispatcher
    {
        private IProcessorSimulator simulator;
        private ConcurrentDictionary<Guid, IProcessor> processors = new ConcurrentDictionary<Guid, IProcessor>();

        public Dispatcher(IProcessorSimulator simulator)
        {
            this.simulator = simulator;
        }
    }
}
