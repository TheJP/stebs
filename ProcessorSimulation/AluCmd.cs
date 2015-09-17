using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessorSimulation
{

    /// <summary>
    /// The possible ALU Operations/Commands
    /// </summary>
    public enum AluCmd
    {
        /// <summary>
        /// No Operation
        /// </summary>
        NOP = 0,

        /// <summary>
        /// Add x and y
        /// </summary>
        ADD = 1,

        /// <summary>
        /// Subtract y from x
        /// </summary>
        SUB = 2,

        /// <summary>
        /// Multiply x and y
        /// </summary>
        MUL = 3,

        /// <summary>
        /// Divide x / y
        /// </summary>
        DIV = 4,

        /// <summary>
        /// Modulo of the division x / y
        /// </summary>
        MOD = 5,

        /// <summary>
        /// Decrement x
        /// </summary>
        DEC = 6,

        /// <summary>
        /// Increment x
        /// </summary>
        INC = 7,

        /// <summary>
        /// Bitwise OR of x and y
        /// </summary>
        OR = 8,

        /// <summary>
        /// Bitwise XOR of x and y
        /// </summary>
        XOR = 9,

        /// <summary>
        /// Bitwise complement of x
        /// </summary>
        NOT = 10,

        /// <summary>
        /// Bitwise AND of x and y
        /// </summary>
        AND = 11,

        /// <summary>
        /// Right shift x
        /// </summary>
        SHR = 12,

        /// <summary>
        /// Left shift x
        /// </summary>
        SHL = 13,

        /// <summary>
        /// Right roll x (shifts and appends the previous LSB on the left)
        /// </summary>
        ROR = 14,

        /// <summary>
        /// Left roll x (shifts and appends the previous MSB on the right)
        /// </summary>
        ROL = 15
    }
}
