using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.Device
{
    /// <summary>
    /// The device manager is a per processor component, which manages the devices of this processor.
    /// </summary>
    public interface IDeviceManager
    {
        /// <summary>
        /// Devices indexed by their slot number.
        /// </summary>
        IReadOnlyDictionary<byte, IDevice> Devices { get; }
        /// <summary>
        /// Adds a device to the processor with the first free slot number.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>Device slot number</returns>
        byte AddDevice(IProcessor processor, IDevice device, IDeviceView view);
        /// <summary>
        /// Adds a device to the processor at the given slot number.
        /// If the slot number is already in use, a free slot number will be chosen.
        /// It's important to always use the returned slot number and not the parameter after this call!
        /// </summary>
        /// <param name="device"></param>
        /// <param name="slot">Prefered slot number</param>
        /// <returns>Device slot number</returns>
        byte AddDevice(IProcessor processor, IDevice device, IDeviceView view, byte slot);
        /// <summary>
        /// Remove the device in the given slot.
        /// </summary>
        /// <param name="slot"></param>
        void RemoveDevice(byte slot);
    }
}
