using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public class Ram : IRam
    {
        private object eventLock = new object();
        private volatile ImmutableDictionary<byte, byte> data;
        public IDictionary<byte, byte> Data
        {
            get { return data; }
        }

        private Action<byte, byte> ramChanged;
        public event Action<byte, byte> RamChanged
        {
            add
            {
                lock (eventLock) { ramChanged += value; }
            }
            remove
            {
                lock (eventLock) { ramChanged -= value; }
            }
        }

        public Ram()
        {
            var dataBuilder = ImmutableDictionary.CreateBuilder<byte, byte>();
            for(byte i = 0; i <= 0xFF; ++i)
            {
                dataBuilder.Add(i, 0);
            }
            data = dataBuilder.ToImmutable();
        }

        public void Set(byte address, byte value)
        {
            data = data.SetItem(address, value);
            NotifyRamChanged(address, value);
        }

        /// <summary>Notifies that the memory of the ram changed.</summary>
        /// <param name="address">Address of the changed cell</param>
        private void NotifyRamChanged(byte address)
        {
            NotifyRamChanged(address, data[address]);
        }

        /// <summary>Notifies that the memory of the ram changed.</summary>
        /// <param name="address">Address of the changed cell</param>
        /// <param name="value">New value of the changed cell</param>
        private void NotifyRamChanged(byte address, byte value)
        {
            Action<byte, byte> handler;
            lock (eventLock)
            {
                handler = ramChanged;
            }
            if(handler != null)
            {
                handler(address, value);
            }
        }

    }
}
