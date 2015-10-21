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
        bool ClearInterruptFlag { get; }
        /// <summary>Affected Flag</summary>
        bool AffectedFlag { get; }
        /// <summary>Defines which alu command to use in alu micro instructions</summary>
        AluCmd AluCommand { get; }

    }
}
