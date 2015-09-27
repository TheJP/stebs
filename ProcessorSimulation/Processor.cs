using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Collections.Immutable;

namespace ProcessorSimulation
{
    public class Processor : IProcessor
    {
        public event Action<IProcessor, IRegister> RegisterChanged;

        private volatile ImmutableDictionary<Registers, IRegister> registers = ImmutableDictionary<Registers, IRegister>.Empty;
        private readonly Func<Registers, byte, IRegister> registerFactory;
        private readonly IAlu alu;

        public Processor(UnityContainer container)
        {
            alu = container.Resolve<IAlu>();
            registerFactory = container.Resolve<Func<Registers, byte, IRegister>>();
            //Initialize the registers dictionary with all existing registers
            foreach (Registers register in Enum.GetValues(typeof(Registers)))
            {
                registers = registers.Add(register, registerFactory(register, 0));
            }
        }
    }
}
