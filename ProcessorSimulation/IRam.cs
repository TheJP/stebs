using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IRam
    {
        /// <summary>
        /// Event that is fired, when the ram is changed.
        /// The 2nd parameter is the address, where the ram was changed.
        /// The 3rd parameter is the value at that ram position.
        /// </summary>
        event Action<byte, byte> RamChanged;

        /// <summary>
        /// Complete memory of the Ram.
        /// </summary>
        IDictionary<byte, byte> Data { get; }

        /// <summary>
        /// Sets the memory cell at given address to given value.
        /// </summary>
        /// <param name="address">Address of the concerned memory cell.</param>
        /// <param name="value">New value, the memory cell should be.</param>
        void Set(byte address, byte value);

    }
}
