using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessorSimulation;
using System.Collections.ObjectModel;

namespace ProcessorDispatcher
{
    public class ChangesCollector : IChangesCollector
    {
        private Dictionary<byte, byte> ramChanges = new Dictionary<byte, byte>();
        public IReadOnlyDictionary<byte, byte> RamChanges => new ReadOnlyDictionary<byte, byte>(ramChanges);
        private Dictionary<Registers, IRegister> registerChanges = new Dictionary<Registers, IRegister>();
        public IReadOnlyDictionary<Registers, IRegister> RegisterChanges => new ReadOnlyDictionary<Registers, IRegister>(registerChanges);
        public bool IsHalted { get; private set; }

        private IProcessor processor = null;

        public void BindTo(IProcessor processor)
        {
            if(this.processor != null) { Unbind(); }
            this.processor = processor;
            processor.RegisterChanged += RegisterChanged;
            processor.Ram.RamChanged += RamChanged;
            processor.Halted += Halted;
            ramChanges.Clear();
            registerChanges.Clear();
            IsHalted = false;
        }

        public void Unbind()
        {
            processor.RegisterChanged -= RegisterChanged;
            processor.Ram.RamChanged -= RamChanged;
            processor.Halted -= Halted;
        }

        private void RegisterChanged(IProcessor processor, IRegister register)
        {
            registerChanges[register.Type] = register;
        }

        private void RamChanged(byte address, byte value)
        {
            ramChanges[address] = value;
        }

        private void Halted(IProcessor processor)
        {
            IsHalted = true;
        }
    }
}
