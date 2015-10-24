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
            this.OpCode = opCode;
            this.MpmAddress = mpmAddress;
            this.Mnemonic = mnemonic;
            this.OperandTypes = operandTypes;
        }
    }
}
