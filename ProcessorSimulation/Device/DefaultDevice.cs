using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.Device
{
    /// <summary>A default device, which can be used to create an own custom device.</summary>
    public abstract class DefaultDevice : IDevice
    {
        private Interrupt interrupt;
        protected IDeviceView View { get; private set; }

        public void Attached(Interrupt interrupt, IDeviceView view)
        {
            this.interrupt = interrupt;
            this.View = view;
            Attached();
        }
        public virtual void Attached() { }
        protected void Interrupt() { interrupt(); }

        public virtual void Detached() { }
        public virtual void Input(byte input) { }
        public virtual byte Output() { return 0; }
        public virtual void Reset() { }
        public virtual void Update(string input) { }
    }
}
