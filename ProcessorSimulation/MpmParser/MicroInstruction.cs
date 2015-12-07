using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Immutable implementation of the IMicroInstruction.
    /// </summary>
    [DebuggerDisplay("Address = {Address}, {Source}->{Destination}, ALU = {AluCommand}, NextAddress = {NextAddress}, JumpCriterion = {JumpCriterion}")]
    public class MicroInstruction : IMicroInstruction
    {
        public int Address { get; }
        public NextAddress NextAddress { get; }
        public bool EnableValue { get; }
        public int Value { get; }
        public JumpCriterion JumpCriterion { get; }
        public bool ClearInterrupt { get; }
        public bool AffectFlags { get; }
        public AluCmd AluCommand { get; }
        public Source Source { get; }
        public Destination Destination { get; }
        public DataInput DataInput { get; }
        public ReadWrite ReadWrite { get; }

        public MicroInstruction(int address, NextAddress nextAddress, bool enableValue,
            int value, JumpCriterion jumpCriterion, bool clearInterrupt,
            bool affected, AluCmd aluCommand, Source source,
            Destination destination, DataInput dataInput, ReadWrite readWrite)
        {
            this.Address = address;
            this.NextAddress = nextAddress;
            this.EnableValue = enableValue;
            this.Value = value;
            this.JumpCriterion = jumpCriterion;
            this.ClearInterrupt = clearInterrupt;
            this.AffectFlags = affected;
            this.AluCommand = aluCommand;
            this.Source = source;
            this.Destination = destination;
            this.DataInput = dataInput;
            this.ReadWrite = readWrite;
        }
    }
}
