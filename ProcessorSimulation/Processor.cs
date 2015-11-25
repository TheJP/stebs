using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading;

namespace ProcessorSimulation
{
    public class Processor : IProcessor
    {
        private object writeLock = new object();

        #region Events
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

        private Action<IProcessor, SimulationState, SimulationStepSize> simulationStateChanged;
        public event Action<IProcessor, SimulationState, SimulationStepSize> SimulationStateChanged
        {
            add
            {
                lock (eventLock) { simulationStateChanged += value; }
            }
            remove
            {
                lock (eventLock) { simulationStateChanged -= value; }
            }
        }

        private Action<IProcessor> macroStepStarted;
        public event Action<IProcessor> MacroStepStarted
        {
            add
            {
                lock (eventLock) { macroStepStarted += value; }
            }
            remove
            {
                lock (eventLock) { macroStepStarted -= value; }
            }
        }

        private Action<IProcessor> instructionStepStarted;
        public event Action<IProcessor> InstructionStepStarted
        {
            add
            {
                lock (eventLock) { instructionStepStarted += value; }
            }
            remove
            {
                lock (eventLock) { instructionStepStarted -= value; }
            }
        }
        #endregion

        private volatile ImmutableDictionary<Registers, IRegister> registers = ImmutableDictionary<Registers, IRegister>.Empty;
        private readonly Func<Registers, uint, IRegister> registerFactory;

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

        /// <summary>Processor constructor with needed dependencies.</summary>
        /// <param name="alu">Alu, which is used for calculations in this processor.</param>
        /// <param name="ram">Ram, which is used as memory for this processor.</param>
        /// <param name="registerFactory">Factory function, that is used to create register instances.</param>
        public Processor(IAlu alu, IRam ram, Func<Registers, uint, IRegister> registerFactory)
        {
            this.Alu = alu;
            this.ram = ram;
            this.registerFactory = registerFactory;
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

        /// <summary>Notifies, that the simulator started/stopped a simulation step.</summary>
        /// <param name="state">State in which the current simulation is.</param>
        /// <param name="stepSize">Step size which is beeing simulated.</param>
        public void NotifySimulationStateChanged(SimulationState state, SimulationStepSize stepSize)
        {
            Action<IProcessor, SimulationState, SimulationStepSize> handler;
            lock (eventLock)
            {
                handler = simulationStateChanged;
            }
            if (handler != null)
            {
                handler(this, state, stepSize);
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
            private Processor Processor { get; }
            public IRamSession RamSession { get; }
            IProcessor IProcessorSession.Processor => Processor;

            private ProcessorSession(Processor processor, IRamSession ram)
            {
                this.Processor = processor;
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
                    Monitor.Exit(Processor.writeLock);
                }
            }

            public void SetRegister(Registers type, uint value)
            {
                var register = Processor.registerFactory(type, value);
                Processor.registers = Processor.registers.SetItem(type, register);
                Processor.NotifyRegisterChanged(register);
            }

            public void SetRegister(IRegister register)
            {
                Processor.registers = Processor.registers.SetItem(register.Type, register);
                Processor.NotifyRegisterChanged(register);
            }
        }
    }
}
