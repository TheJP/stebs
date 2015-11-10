using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Possible operand types for assembler instructions.
    /// </summary>
    public class OperandType
    {
        /// <summary>Constant value from the memory</summary>
        /// <remarks>const</remarks>
        public static OperandType Const { get; } = new OperandType("const", "Const");
        /// <summary>Used for relative addressing. (E.g. addressing in the JMP instruction)</summary>
        /// <remarks>offset</remarks>
        public static OperandType Offset { get; } = new OperandType("offset", "Offset");
        /// <summary>Memory address</summary>
        /// <remarks>addr</remarks>
        public static OperandType Address { get; } = new OperandType("addr", "Address");
        /// <summary>Operand describes one of the working registers (AL, BL, ...) which can be described by the SEL register</summary>
        /// <remarks>reg</remarks>
        public static OperandType Register { get; } = new OperandType("reg", "Register");
        /// <summary>Memory address, which is described by a constant value from the memory</summary>
        /// <remarks>|addr|</remarks>
        public static OperandType IndirectAddress { get; } = new OperandType("|addr|", "IndirectAddress");
        /// <summary>Memory address, which is described by one of the working registers. See <see cref="OperandType.Register"/></summary>
        /// <remarks>|reg|</remarks>
        public static OperandType RegisterIndirectAddress { get; } = new OperandType("|reg|", "RegisterIndirectAddress");
        /// <summary>Used for absolute addressing. (E.g. addressing in the JPA instruction)</summary>
        /// <remarks>absolute</remarks>
        public static OperandType Absolute { get; } = new OperandType("absolute", "Absolute");

        private static Dictionary<string, OperandType> conversions = new Dictionary<string, OperandType>()
        {
            [Const.ShortOperand] = Const,
            [Offset.ShortOperand] = Offset,
            [Address.ShortOperand] = Address,
            [Register.ShortOperand] = Register,
            [IndirectAddress.ShortOperand] = IndirectAddress,
            [RegisterIndirectAddress.ShortOperand] = RegisterIndirectAddress,
            [Absolute.ShortOperand] = Absolute
        };

        public string Operand { get; }
        public string ShortOperand { get; }
        private OperandType(string shortOperand, string operand)
        {
            this.ShortOperand = shortOperand;
            this.Operand = operand;
        }

        public static OperandType FromString(string operand) => conversions[operand.Trim()];
        public static IImmutableList<OperandType> FromStrings(params string[] operands) => operands.Select(FromString).ToImmutableList();
    }
}