using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Possible jump criteria in a jump micro instruction.
    /// </summary>
    public enum JumpCriterion
    {
        Empty = 0,
        /// <summary>
        /// Not zero
        /// </summary>
        NotZero = 1,
        /// <summary>
        /// No overflow
        /// </summary>
        NoOverflow = 2,
        /// <summary>
        /// Not signed
        /// </summary>
        NotSigned = 3,
        /// <summary>
        /// Zero
        /// </summary>
        Zero = 4,
        /// <summary>
        /// Overflow
        /// </summary>
        Overflow = 5,
        /// <summary>
        /// Signed
        /// </summary>
        Signed = 6
    }
}
