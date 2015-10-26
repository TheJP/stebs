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
        ///
        /// </summary>
        Data = 20,
        SEL_REF,
        SR,
        Empty
    }
}
