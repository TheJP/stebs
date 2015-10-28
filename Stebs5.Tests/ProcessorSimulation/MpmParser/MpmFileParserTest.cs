using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessorSimulation.MpmParser;
using System.Collections.Immutable;

namespace Stebs5.Tests.ProcessorSimulation.MpmParser
{
    [TestClass]
    public class MpmFileParserTest
    {
        private IMpmFileParser parser;

        [TestInitialize]
        public void Setup()
        {
            parser = new MpmFileParser();
        }

        [TestMethod]
        public void TestParseInstructions0()
        {
            string testCase = "1;1;XXX";
            var result = parser.ParseInstructions(testCase);
            var expected = new Instruction[] { new Instruction(1, 1, "XXX", ImmutableList<OperandType>.Empty) };
            Helper.EnumerableEqual(expected, result);
        }

        [TestMethod]
        public void TestParseInstructions1()
        {
            string testCase = "0F;10;ABC\n10;10;XYZ reg\r\n11;1f;DEF |addr|,|reg|";
            var result = parser.ParseInstructions(testCase);
            var expected = new Instruction[] {
                new Instruction(0x10, 0x0f, "ABC", ImmutableList<OperandType>.Empty),
                new Instruction(0x10, 0x10, "XYZ", ImmutableList.Create(OperandType.Register)),
                new Instruction(0x1f, 0x11, "DEF", ImmutableList.Create(OperandType.IndirectAddress, OperandType.RegisterIndirectAddress))
            };
            Helper.EnumerableEqual(expected, result);
        }

        [TestMethod]
        public void TestParseInstructionsFile()
        {
            var testCase = "Resources\\INSTRUCTION.data";
            var result = parser.ParseInstructionsFile(testCase);
            var expected = new Instruction[] {
                new Instruction(0x00, 0x00, "AAA", ImmutableList<OperandType>.Empty),
                new Instruction(0x01, 0x01, "BBB", ImmutableList.Create(OperandType.Const)),
                new Instruction(0x02, 0x02, "CCC", ImmutableList.Create(OperandType.Offset)),
                new Instruction(0x03, 0x03, "DDD", ImmutableList.Create(OperandType.Address)),
                new Instruction(0x04, 0x04, "EEE", ImmutableList.Create(OperandType.Register)),
                new Instruction(0x05, 0x05, "FFF", ImmutableList.Create(OperandType.IndirectAddress)),
                new Instruction(0x06, 0x06, "GGG", ImmutableList.Create(OperandType.RegisterIndirectAddress)),
                new Instruction(0x07, 0x07, "HHH", ImmutableList.Create(OperandType.Absolute)),
                new Instruction(0x08, 0x08, "III", ImmutableList.Create(OperandType.Const, OperandType.Offset)),
                new Instruction(0x09, 0x09, "JJJ", ImmutableList.Create(OperandType.Address, OperandType.Register)),
                new Instruction(0x0a, 0x0a, "KKK", ImmutableList.Create(OperandType.IndirectAddress, OperandType.RegisterIndirectAddress)),
                new Instruction(0x0b, 0x0b, "LLL", ImmutableList.Create(OperandType.Absolute, OperandType.Absolute)),
            };
            Helper.EnumerableEqual(expected, result);
        }
    }
}
