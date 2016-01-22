using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.Device
{
    public class DeviceManager : IDeviceManager
    {
        private const string MaxDevicedReached = "This processor already has the maximal number of devices attached.";
        private byte nextSlot = 1;
        private readonly Dictionary<byte, IDevice> devices = new Dictionary<byte, IDevice>();
        public IReadOnlyDictionary<byte, IDevice> Devices => new ReadOnlyDictionary<byte, IDevice>(devices);

        public byte AddDevice(IDevice device)
        {
            if(devices.Count >= byte.MaxValue) { throw new ArgumentException(MaxDevicedReached); }
            while (devices.ContainsKey(nextSlot)) { ++nextSlot; }
            return AddDevice(device, nextSlot++);
        }

        public byte AddDevice(IDevice device, byte slot)
        {
            if (devices.ContainsKey(slot)) { return AddDevice(device); }
            else
            {
                devices[slot] = device;
                device.Attached();
                return slot;
            }
        }

        public void RemoveDevice(byte slot)
        {
            var device = devices[slot];
            devices.Remove(slot);
            device.Detached();
        }
    }
}
