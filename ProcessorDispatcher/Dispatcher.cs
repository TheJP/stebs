using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ProcessorSimulation;
using Microsoft.Practices.Unity;

namespace ProcessorDispatcher
{
    public class Dispatcher : IDispatcher
    {
        private IProcessorSimulator simulator;
        private UnityContainer container;
        private ConcurrentDictionary<Guid, IProcessor> processors = new ConcurrentDictionary<Guid, IProcessor>();

        public Dispatcher(IProcessorSimulator simulator, UnityContainer container)
        {
            this.simulator = simulator;
            this.container = container;
        }

        public KeyValuePair<Guid, IProcessor>? CreateProcessor()
        {
            var guid = Guid.NewGuid();
            var processor = container.Resolve<IProcessor>();
            if(processors.TryAdd(guid, processor))
            {
                return new KeyValuePair<Guid, IProcessor>(guid, processor);
            }
            return null;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
