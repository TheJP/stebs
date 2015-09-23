using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace ProcessorSimulation
{
    public class Processor : IProcessor
    {
        public event Action<IProcessor, IRegister> RegisterChanged;

        private Dictionary<Registers, IRegister> registers = new Dictionary<Registers, IRegister>();
        private IAlu Alu { get; set; }

        public Processor(UnityContainer container)
        {
            Alu = container.Resolve<IAlu>();
            Func<Registers, IRegister> registerFactory = container.Resolve<Func<Registers, IRegister>>();
            //Initialize the registers dictionary with all existing registers
            foreach (Registers register in Enum.GetValues(typeof(Registers)))
            {
                registers.Add(register, registerFactory(register));
            }
        }
    }
}
