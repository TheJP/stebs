using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IReadOnlyRam
    {
        /// <summary>
        /// Event that is fired, when the ram is changed.
        /// The 1st parameter is the address, where the ram was changed.
        /// The 2nd parameter is the value at that ram position.
        /// </summary>
        event Action<byte, byte> RamChanged;

        /// <summary>
        /// Complete memory of the Ram.
        /// </summary>
        IDictionary<byte, byte> Data { get; }
    }
}
