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
        /// The 1st parameter is the address, where the ram was changed.
        /// The 2nd parameter is the value at that ram position.
        /// </summary>
        event Action<byte, byte> RamChanged;

        /// <summary>
        /// Complete memory of the Ram.
        /// </summary>
        IDictionary<byte, byte> Data { get; }

        /// <summary>Create session, with which the ram state can be modified.</summary>
        /// <returns>Session instance</returns>
        /// <remarks>This method can block, because only one session should exist and it should be used by one thread only.</remarks>
        IRamSession CreateSession();
    }
}
