using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.Device
{
    /// <summary>
    /// A device is a sensor or actor (or both), which can be connected to a processor.
    /// 
    /// A device can interact with the processor in three ways:
    /// 1. Take input, which is sent from the processor. (actor)
    /// 2. Output data to the processor on request. (sensor)
    /// 3. Set the interrupt flag.
    /// Points 1. and 2. have passive behavior: They are only called if the assembly commands IN or OUT are executed.
    /// Setting the interrupt flag, however, can be done at any time.
    /// 
    /// The life cycle of a device is the following. (Most states corresponds to a method)
    /// "Created" -> "Attached" -> "Reset" -> "Reset" -> ... -> "Reset" -> "Detached" -> "Destroyed"
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Called after a device is attached to a processor.
        /// </summary>
        void Attached();
        /// <summary>
        /// Called after a device is detached from the processor.
        /// It can be assumed, that the device will not be used any further. (E.g. Resources can be released at this point.)
        /// </summary>
        void Detached();
        /// <summary>
        /// Called after the processor, to which the device was attached, did get a reset.
        /// </summary>
        void Reset();
        /// <summary>
        /// This method is called, when data is written to the device.
        /// (This method corresponds with the assembly mnemonic OUT.)
        /// </summary>
        /// <param name="input">Data which is written to the device.</param>
        void Input(byte input);
        /// <summary>
        /// This method is called, when data is read from the device.
        /// (This method corresponds with the assembly mnemonic IN.)
        /// </summary>
        /// <returns>Data which is read from the device.</returns>
        byte Output();
    }
}
