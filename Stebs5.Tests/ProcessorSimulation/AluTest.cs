using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessorSimulation;
using System.Collections.Generic;
using static ProcessorSimulation.AluCmd;

namespace Stebs5.Tests.ProcessorSimulation
{
    [TestClass]
    public class AluTest
    {

        private Alu alu;

        [TestInitialize]
        public void Setup()
        {
            alu = new Alu();
        }

        [TestMethod]
        public void TestExecute()
        {
            //Test cases in tuples: command, x, y, expected result
            Func<AluCmd, byte, byte, byte, Tuple<AluCmd, byte, byte, byte>> test = Tuple.Create;
            var testCases = new List<Tuple<AluCmd, byte, byte, byte>>()
            {
                test(NOP, 0, 0, 0),
                //ADD
                test(ADD, 0, 0, 0),
                test(ADD, 5, 0, 5),
                test(ADD, 0, 5, 5),
                test(ADD, 5, 5, 10),
                test(ADD, 10, 0xFB, 5),
                test(ADD, 5, 0xF6, 0xFB),
                test(ADD, 0xFB, 10, 5),
                //SUB
                test(SUB, 0, 0, 0),
                test(SUB, 10, 0, 10),
                test(SUB, 10, 5, 5),
                test(SUB, 0, 10, 0xF6),
                test(SUB, 5, 5, 0),
                test(SUB, 0xFF, 0, 0xFF),
                test(SUB, 0xFF, 0xFF, 0),
                //MUL
                test(MUL, 0, 0, 0),
                test(MUL, 1, 0, 0),
                test(MUL, 0, 1, 0),
                test(MUL, 33, 1, 33),
                test(MUL, 1, 33, 33),
                test(MUL, 4, 16, 64),
                test(MUL, 0x80, 2, 0),
                test(MUL, 0xFF, 2, 0xFE),
                //DIV
                test(DIV, 0, 1, 0),
                test(DIV, 5, 1, 5),
                test(DIV, 10, 2, 5),
                test(DIV, 0xF6, 1, 0xF6),
                test(DIV, 0xF6, 2, 0x7B), //Unsigned Division!
                test(DIV, 10, 10, 1),
                test(DIV, 20, 10, 2),
                test(DIV, 10, 0xFF, 0),
                test(DIV, 1, 2, 0),
                test(DIV, 10, 11, 0),
            };
            //Test each case
            foreach(var testCase in testCases)
            {
                Assert.AreEqual(
                    testCase.Item4,
                    alu.Execute(testCase.Item1, testCase.Item2, testCase.Item3),
                    $"{testCase.Item1}({testCase.Item2}, {testCase.Item3})");
            }
        }

        [TestMethod]
        public void TestDivisionByZero()
        {
            try
            {
                alu.Execute(DIV, 5, 0);
                Assert.Fail("Division by 0 doesn't throw an exception");
            }
            catch (DivideByZeroException) { }
        }
    }
}
