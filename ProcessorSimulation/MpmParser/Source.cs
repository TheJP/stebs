using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    public enum Source
    {
        /// <summary>
        /// Instruction pointer
        /// </summary>
        IP = Registers.IP,

        /// <summary>
        /// Memory address register
        /// </summary>
        MAR = Registers.MAR,

        /// <summary>
        /// Memory buffer register
        /// </summary>
        MBR = Registers.MBR,

        /// <summary>
        /// Memory data register
        /// </summary>
        MDR = Registers.MDR,

        /// <summary>
        /// ALU result register
        /// </summary>
        RES = Registers.RES,

        /// <summary>
        /// Status register
        /// </summary>
        Status = Registers.Status,

        /// <summary>
        /// The source is the RAM or the IO (devices),
        /// depending on the DataInput flag of the micro program memory entry.
        /// </summary>
        Data = 22,

        /// <summary>
        /// Defines that the working registers (AL, BL, ...) should be used.
        /// The register is choosen by the value in the SEL register.
        /// </summary>
        SELReferenced = 23,

        /// <summary>
        /// No source defined: The data bus will be the defined value of the mpm entry (if enable value is set) or 0.
        /// </summary>
        Empty = 24
    }
}
