using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    /// <summary>
    /// Facory, which allows the creation of <see cref="IRegister"/>s.
    /// </summary>
    public delegate IRegister RegisterFactory(Registers type, uint value);

    /// <summary>
    /// Immutable register in a processor.
    /// Caution: The type of the value field is uint, but mostly byte values are stored.
    /// This is because there exist special cases, with registers that need to store bigger values.
    /// (E.g. MIP is a 12-bit register.)
    /// </summary>
    public interface IRegister
    {
        Registers Type { get; }
        /// <summary>Value, which is stored by the register</summary>
        uint Value { get; }
    }
}
