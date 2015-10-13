using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public class Ram : IRam
    {
        private object eventLock = new object();
        private object writeLock = new object();
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

        public IRamSession createSession()
        {
            return RamSession.createSession(this);
        }

        public class RamSession : IRamSession
        {
            private bool disposed = false;
            private Ram Session { get; }
            private RamSession(Ram ram)
            {
                Session = ram;
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    Monitor.Exit(Session.writeLock);
                }
            }

            public static RamSession createSession(Ram ram)
            {
                Monitor.Enter(ram.writeLock);
                return new RamSession(ram);
            }

            public void Set(byte address, byte value)
            {
                Session.data = Session.data.SetItem(address, value);
                Session.NotifyRamChanged(address, value);
            }

        }
    }
}
