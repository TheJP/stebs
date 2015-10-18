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
        private IRam ram;
        public IReadOnlyRam Ram
        {
            get { return new ReadOnlyRam(ram); }
        }
        public IDictionary<Registers, IRegister> Registers
        {
            get { return registers; }
        }
        /// <summary>Creates a processor. The different parts are created by resolving unity dependencies.</summary>
        /// <param name="container">Container that allows the resolving of required dependencies.</param>
        public Processor(UnityContainer container)
        {
            Alu = container.Resolve<IAlu>();
            ram = container.Resolve<IRam>();
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

        /// <summary>Proxy Pattern to protect write access to the ram.</summary>
        /// <remarks>
        /// This prevents deadlocks, because the ram is locking and often processor and ram sessions are required.
        /// This protection ensures the order in which locks (sessions) to processor and ram are acquired.
        /// </remarks>
        public sealed class ReadOnlyRam : IReadOnlyRam
        {
            private IRam Ram { get; }

            public IDictionary<byte, byte> Data
            {
                get { return Ram.Data; }
            }

            public event Action<byte, byte> RamChanged
            {
                add { Ram.RamChanged += value; }
                remove { Ram.RamChanged -= value; }
            }

            public ReadOnlyRam(IRam ram)
            {
                this.Ram = ram;
            }
        }

        public sealed class ProcessorSession : IProcessorSession
        {
            private bool disposed = false;
            private Processor Session { get; }
            public IRamSession RamSession { get; }

            private ProcessorSession(Processor processor, IRamSession ram)
            {
                Session = processor;
                this.RamSession = ram;
            }

            public static ProcessorSession createSession(Processor processor)
            {
                Monitor.Enter(processor.writeLock);
                return new ProcessorSession(processor, processor.ram.createSession());
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    RamSession.Dispose();
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
