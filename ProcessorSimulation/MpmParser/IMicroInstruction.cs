using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// One Instruction of the Micro Program Memory (MPM).
    /// </summary>
    public interface IMicroInstruction
    {
        /// <summary>Absolute address in the memory</summary>
        int Address { get; }
        /// <summary>Defines how to calculate the next micro program memory address</summary>
        NextAddress NextAddress { get; }
        /// <summary>Enable value</summary>
        bool EnableValue { get; }
        /// <summary>Offset, address or value</summary>
        int Value { get; }
        /// <summary>Criterion which is used in jump instructions</summary>
        JumpCriterion JumpCriterion { get; }
        /// <summary>Clear interrupt flag</summary>
        bool ClearInterrupt { get; }
        /// <summary>Determines, if the ALU instruction (if not <see cref="AluCmd.NOP"/>) affects the status flags</summary>
        bool AffectFlags { get; }
        /// <summary>Defines which alu command to use in alu micro instructions</summary>
        AluCmd AluCommand { get; }
        /// <summary>Source where the data is read from with this micro instruction</summary>
        Source Source { get; }
        /// <summary>Destination where the data is written to with this micro instruction</summary>
        Destination Destination { get; }
        /// <summary>Defines where the input of the data comes from in this micro instruction</summary>
        DataInput DataInput { get; }
        /// <summary>Controls the flow, which is either read or write</summary>
        ReadWrite ReadWrite { get; }
    }
}
