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
            string testCase = "0F;10;ABC\n10;10;XYZ reg\r\n11;11;DEF |addr|,|reg|";
            var result = parser.ParseInstructions(testCase);
            var expected = new Instruction[] {
                new Instruction(0x10, 0x0f, "ABC", ImmutableList<OperandType>.Empty),
                new Instruction(0x10, 0x10, "XYZ", ImmutableList.Create(OperandType.Register)),
                new Instruction(0x11, 0x11, "DEF", ImmutableList.Create(OperandType.IndirectAddress, OperandType.RegisterIndirectAddress))
            };
            Helper.EnumerableEqual(expected, result);
        }
    }
}
