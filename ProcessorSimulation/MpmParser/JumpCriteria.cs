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
        EMPTY = 0,
        /// <summary>
        /// Not zero
        /// </summary>
        NZ = 1,
        /// <summary>
        /// No overflow
        /// </summary>
        NO = 2,
        /// <summary>
        /// Not signed
        /// </summary>
        NS = 3,
        /// <summary>
        /// Zero
        /// </summary>
        Z = 4,
        /// <summary>
        /// Overflow
        /// </summary>
        O = 5,
        /// <summary>
        /// Signed
        /// </summary>
        S = 6
    }
}
