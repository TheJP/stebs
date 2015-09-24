using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    /// <summary>
    /// Factory which has to be used to create processors.
    /// This factory is important, because it sets up the unity dependency injection, which is used in the processor.
    /// </summary>
    public class ProcessorFactory : IProcessorFactory
    {
        private UnityContainer container = new UnityContainer();

        public ProcessorFactory()
        {
            container
                .RegisterInstance<Func<Registers, IRegister>>(type => new Register(type))
                .RegisterInstance<IAlu>(new Alu(container));
        }

        public IProcessor CreateProcessor()
        {
            return new Processor(container);
        }
    }
}
