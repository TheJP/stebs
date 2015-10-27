using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Represents a typed instruction as the opcode decoder would see it.
    /// </summary>
    public interface IInstruction : IEquatable<IInstruction>
    {
        /// <summary>Machine code representation of this instruction</summary>
        byte OpCode { get; }
        /// <summary>Address of the start of this instruction in the micro program memory</summary>
        int MpmAddress { get; }
        /// <summary>Assembly code representation of this instruction</summary>
        string Mnemonic { get; }
        /// <summary>Operands of this instruction</summary>
        IImmutableList<OperandType> OperandTypes { get; }
    }
}