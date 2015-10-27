using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    public class Instruction : IInstruction
    {
        public byte OpCode { get; }
        public int MpmAddress { get; }
        public string Mnemonic { get; }
        public IImmutableList<OperandType> OperandTypes { get; }

        public Instruction(byte opCode, int mpmAddress, string mnemonic, IImmutableList<OperandType> operandTypes)
        {
            if(mnemonic == null || operandTypes == null) { throw new ArgumentNullException(); }
            this.OpCode = opCode;
            this.MpmAddress = mpmAddress;
            this.Mnemonic = mnemonic;
            this.OperandTypes = operandTypes;
        }

        public bool Equals(IInstruction instruction) => Equals((object)instruction);
        public override bool Equals(object obj)
        {
            //Default way to implement equality
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }
            var other = (Instruction)obj;
            return OpCode == other.OpCode && MpmAddress == other.MpmAddress &&
                string.Equals(Mnemonic, other.Mnemonic) && OperandTypes.SequenceEqual(other.OperandTypes);
        }

        public override int GetHashCode() =>
            OpCode ^ MpmAddress ^ Mnemonic.GetHashCode() ^ OperandTypes.Select(o => o.GetHashCode()).Aggregate((a, b) => a ^ b);
    }
}
