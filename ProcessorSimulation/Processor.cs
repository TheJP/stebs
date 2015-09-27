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
        //Because processor events are accessed by multiple threads custom locking has to be implemented
        //to guarantee thread safety. (See also the delegate chapter of 'C# in depth')
        private object eventLock = new object();

        private Action<IProcessor, IRegister> registerChanged;
        public event Action<IProcessor, IRegister> RegisterChanged
        {
            add
            {
                lock (eventLock) { registerChanged += value; }
            }
            remove
            {
                lock (eventLock) { registerChanged -= value; }
            }
        }

        private volatile ImmutableDictionary<Registers, IRegister> registers = ImmutableDictionary<Registers, IRegister>.Empty;
        private readonly Func<Registers, byte, IRegister> registerFactory;
        private readonly IAlu alu;
        private readonly IRam ram;

        public Processor(UnityContainer container)
        {
            alu = container.Resolve<IAlu>();
            ram = container.Resolve<IRam>();
            registerFactory = container.Resolve<Func<Registers, byte, IRegister>>();
            //Initialize the registers dictionary with all existing registers
            foreach (Registers register in Enum.GetValues(typeof(Registers)))
            {
                registers = registers.Add(register, registerFactory(register, 0));
            }
        }

        public void NotifyRegisterChanged(IRegister register)
        {
            Action<IProcessor, IRegister> handler;
            lock (eventLock)
            {
                handler = registerChanged;
            }
            if(handler != null)
            {
                //Call handler outside of the lock, so called handle methods will not caue a deadlock.
                //This is safe because delegates are immutable.
                handler(this, register);
            }
        }
    }
}
