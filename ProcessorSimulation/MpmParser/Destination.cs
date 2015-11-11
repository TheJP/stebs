using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    public enum Destination
    {
        /// <summary>
        /// Instruction pointer
        /// </summary>
        IP = Registers.IP,
        /// <summary>
        /// Instruction register
        /// </summary>
        IR = Registers.IR,
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
        /// Selection of the data register
        /// </summary>
        SEL = Registers.SEL,
        /// <summary>
        /// ALU register X
        /// </summary>
        X = Registers.X,
        /// <summary>
        /// ALU register Y
        /// </summary>
        Y = Registers.Y,
        /// <summary>
        /// Status register
        /// </summary>
        Status = Registers.Status,
        SEL_REF,
        Empty
    }
}
