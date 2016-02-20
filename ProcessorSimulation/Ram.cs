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

        /// <summary>Size of the ram in byte</summary>
        public const int RamSize = 256;

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
            for(int i = 0; i < RamSize; ++i)
            {
                dataBuilder.Add((byte)i, 0);
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

        public IRamSession CreateSession()
        {
            return RamSession.CreateSession(this);
        }

        public sealed class RamSession : IRamSession
        {
            private bool disposed = false;
            private Ram Ram { get; }
            IRam IRamSession.Ram => Ram;

            private RamSession(Ram ram)
            {
                this.Ram = ram;
            }

            ~RamSession() { Dispose(); }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    Monitor.Exit(Ram.writeLock);
                }
            }

            public static RamSession CreateSession(Ram ram)
            {
                Monitor.Enter(ram.writeLock);
                return new RamSession(ram);
            }

            public void Set(byte address, byte value)
            {
                if (disposed) { throw new InvalidOperationException("Mutative access to closed session"); }
                Ram.data = Ram.data.SetItem(address, value);
                Ram.NotifyRamChanged(address, value);
            }

            public void Set(byte[] values)
            {
                if (disposed) { throw new InvalidOperationException("Mutative access to closed session"); }
                if (values.Length != RamSize) { throw new ArgumentException($"{values.Length} bytes should be written to the Ram, but the Ram has size {RamSize}"); }
                var dataBuilder = ImmutableDictionary.CreateBuilder<byte, byte>();
                for (int i = 0; i < RamSize; ++i)
                {
                    dataBuilder.Add((byte)i, values[i]);
                    Ram.NotifyRamChanged((byte)i, values[i]);
                }
                Ram.data = dataBuilder.ToImmutable();
            }
        }
    }
}
