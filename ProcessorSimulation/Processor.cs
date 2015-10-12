using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Collections.Immutable;
using System.Threading;

namespace ProcessorSimulation
{
    public class Processor : IProcessor
    {
        //Because processor events are accessed by multiple threads custom locking has to be implemented
        //to guarantee thread safety. (See also the delegate chapter of 'C# in depth')
        private object eventLock = new object();
        private object writeLock = new object();

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

        public IAlu Alu { get; }
        public IRam Ram { get; }
        public IDictionary<Registers, IRegister> Registers
        {
            get { return registers; }
        }
        /// <summary>Creates a processor. The different parts are created by resolving unity dependencies.</summary>
        /// <param name="container">Container that allows the resolving of required dependencies.</param>
        public Processor(UnityContainer container)
        {
            Alu = container.Resolve<IAlu>();
            Ram = container.Resolve<IRam>();
            registerFactory = container.Resolve<Func<Registers, byte, IRegister>>();
            //Initialize the registers dictionary with all existing registers
            registers = registers.AddRange(
                ((Registers[])Enum.GetValues(typeof(Registers)))
                .Select(type => new KeyValuePair<Registers, IRegister>(type, registerFactory(type, 0))));
        }

        /// <summary>Notifies that the register with the given type changed.</summary>
        /// <remarks>The changed register has to be entered to the registers data structure beforehand.</remarks>
        /// <param name="register">Type of the changed register.</param>
        private void NotifyRegisterChanged(Registers register)
        {
            NotifyRegisterChanged(registers[register]);
        }

        /// <summary>Notifies, that the given register changed.</summary>
        /// <param name="register">New register</param>
        private void NotifyRegisterChanged(IRegister register)
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

        public IProcessorSession createSession()
        {
            return ProcessorSession.createSession(this);
        }

        public class ProcessorSession : IProcessorSession
        {
            private bool disposed = false;
            private Processor processor;

            private Processor Session { get; }

            private ProcessorSession(Processor processor)
            {
                Session = processor;
            }

            public static ProcessorSession createSession(Processor processor)
            {
                Monitor.Enter(processor.writeLock);
                return new ProcessorSession(processor);
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    Monitor.Exit(Session.writeLock);
                }
            }

            public void SetRegister(Registers type, byte value)
            {
                var register = Session.registerFactory(type, value);
                Session.registers = Session.registers.SetItem(type, register);
                Session.NotifyRegisterChanged(register);
            }
        }
    }
}
