using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IRamSession : IDisposable
    {
        /// <summary>
        /// Sets the memory cell at given address to given value.
        /// </summary>
        /// <param name="address">Address of the concerned memory cell.</param>
        /// <param name="value">New value, the memory cell should be.</param>
        void Set(byte address, byte value);

        /// <summary>
        /// Sets the complete memory to given values.
        /// </summary>
        /// <param name="values">New values of the Ram.</param>
        /// <exception cref="ArgumentException">If the length of the values array does not match the size of the <see cref="IRam"/>.</exception>
        void Set(byte[] values);

        /// <summary>
        /// Access to the Ram encasuled in the session for read operations.
        /// </summary>
        IRam Ram { get; }
    }
}
