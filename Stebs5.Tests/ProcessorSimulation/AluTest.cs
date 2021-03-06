﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessorSimulation;
using System.Collections.Generic;
using static ProcessorSimulation.AluCmd;
using Microsoft.Practices.Unity;

namespace Stebs5.Tests.ProcessorSimulation
{
    [TestClass]
    public class AluTest
    {

        private Alu alu;

        [TestInitialize]
        public void Setup()
        {
            alu = new Alu((registers, value) => new Register(registers, value));
        }

        [TestMethod]
        public void TestExecute()
        {
            //Test cases in tuples: command, x, y, expected result
            Func<AluCmd, byte, byte, byte, byte, Tuple<AluCmd, byte, byte, byte, byte>> test = Tuple.Create;
            Func<int, int, int, byte> status = (s, o, z) => (byte)((s << 3) | (o << 2) | (z << 1));
            var testCases = new List<Tuple<AluCmd, byte, byte, byte, byte>>()
            {
                test(NOP, 0, 0, 0, status(0, 0, 1)),
                //ADD
                test(ADD, 0, 0, 0, status(0, 0, 1)),
                test(ADD, 5, 0, 5, status(0, 0, 0)),
                test(ADD, 0, 5, 5, status(0, 0, 0)),
                test(ADD, 5, 5, 10, status(0, 0, 0)),
                test(ADD, 10, 0xFB, 5, status(0, 1, 0)),
                test(ADD, 5, 0xF6, 0xFB, status(1, 0, 0)),
                test(ADD, 0xFB, 10, 5, status(0, 1, 0)),
                //SUB
                test(SUB, 0, 0, 0, status(0, 0, 1)),
                test(SUB, 10, 0, 10, status(0, 0, 0)),
                test(SUB, 10, 5, 5, status(0, 0, 0)),
                test(SUB, 0, 10, 0xF6, status(1, 1, 0)),
                test(SUB, 5, 5, 0, status(0, 0, 1)),
                test(SUB, 0xFF, 0, 0xFF, status(1, 0, 0)),
                test(SUB, 0xFF, 0xFF, 0, status(0, 0, 1)),
                //MUL
                test(MUL, 0, 0, 0, status(0, 0, 1)),
                test(MUL, 1, 0, 0, status(0, 0, 1)),
                test(MUL, 0, 1, 0, status(0, 0, 1)),
                test(MUL, 33, 1, 33, status(0, 0, 0)),
                test(MUL, 1, 33, 33, status(0, 0, 0)),
                test(MUL, 4, 16, 64, status(0, 0, 0)),
                test(MUL, 0x80, 2, 0, status(0, 1, 1)),
                test(MUL, 0xFF, 2, 0xFE, status(1, 1, 0)),
                //DIV
                test(DIV, 0, 1, 0, status(0, 0, 1)),
                test(DIV, 5, 1, 5, status(0, 0, 0)),
                test(DIV, 10, 2, 5, status(0, 0, 0)),
                test(DIV, 0xF6, 1, 0xF6, status(1, 0, 0)),
                test(DIV, 0xF6, 2, 0x7B, status(0, 0, 0)), //Unsigned Division!
                test(DIV, 10, 10, 1, status(0, 0, 0)),
                test(DIV, 20, 10, 2, status(0, 0, 0)),
                test(DIV, 10, 0xFF, 0, status(0, 0, 1)),
                test(DIV, 1, 2, 0, status(0, 0, 1)),
                test(DIV, 10, 11, 0, status(0, 0, 1)),
                //MOD
                test(MOD, 0, 1, 0, status(0, 0, 1)),
                test(MOD, 0, 5, 0, status(0, 0, 1)),
                test(MOD, 5, 5, 0, status(0, 0, 1)),
                test(MOD, 10, 5, 0, status(0, 0, 1)),
                test(MOD, 12, 5, 2, status(0, 0, 0)),
                test(MOD, 0xFF, 0x10, 0x0F, status(0, 0, 0)),
                test(MOD, 0xAC, 0x10, 0x0C, status(0, 0, 0)),
                test(MOD, 33, 66, 33, status(0, 0, 0)),
                test(MOD, 0xFF, 0xFF, 0, status(0, 0, 1)),
                //DEC
                test(DEC, 1, 0, 0, status(0, 0, 1)),
                test(DEC, 2, 1, 1, status(0, 0, 0)),
                test(DEC, 0xFF, 2, 0xFE, status(1, 0, 0)),
                test(DEC, 0x25, 3, 0x24, status(0, 0, 0)),
                test(DEC, 0, 4, 0xFF, status(1, 1, 0)),
                //INC
                test(INC, 1, 0, 2, status(0, 0, 0)),
                test(INC, 2, 1, 3, status(0, 0, 0)),
                test(INC, 0xFF, 2, 0, status(0, 1, 1)),
                test(INC, 0x25, 3, 0x26, status(0, 0, 0)),
                test(INC, 0, 4, 1, status(0, 0, 0)),
                //OR
                test(OR, 0, 0, 0, status(0, 0, 1)),
                test(OR, 0, 1, 1, status(0, 0, 0)),
                test(OR, 1, 0, 1, status(0, 0, 0)),
                test(OR, 1, 1, 1, status(0, 0, 0)),
                test(OR, 0, 0xFF, 0xFF, status(1, 0, 0)),
                test(OR, 0xFF, 0, 0xFF, status(1, 0, 0)),
                test(OR, 0xFF, 0xFF, 0xFF, status(1, 0, 0)),
                test(OR, 0xF0, 0x0F, 0xFF, status(1, 0, 0)),
                test(OR, 0x3C, 0xC3, 0xFF, status(1, 0, 0)),
                //XOR
                test(XOR, 0, 0, 0, status(0, 0, 1)),
                test(XOR, 0, 1, 1, status(0, 0, 0)),
                test(XOR, 1, 0, 1, status(0, 0, 0)),
                test(XOR, 1, 1, 0, status(0, 0, 1)),
                test(XOR, 0, 0xFF, 0xFF, status(1, 0, 0)),
                test(XOR, 0xFF, 0, 0xFF, status(1, 0, 0)),
                test(XOR, 0xFF, 0xFF, 0, status(0, 0, 1)),
                test(XOR, 0xF0, 0x0F, 0xFF, status(1, 0, 0)),
                test(XOR, 0x3C, 0xC3, 0xFF, status(1, 0, 0)),
                //NOT
                test(NOT, 0, 0, 0xFF, status(1, 0, 0)),
                test(NOT, 1, 1, 0xFE, status(1, 0, 0)),
                test(NOT, 0xFE, 2, 1, status(0, 0, 0)),
                test(NOT, 0xFF, 3, 0, status(0, 0, 1)),
                test(NOT, 0x0F, 4, 0xF0, status(1, 0, 0)),
                test(NOT, 0xF0, 5, 0x0F, status(0, 1, 0)),
                //AND
                test(AND, 0, 0, 0, status(0, 0, 1)),
                test(AND, 0, 1, 0, status(0, 0, 1)),
                test(AND, 1, 0, 0, status(0, 0, 1)),
                test(AND, 1, 1, 1, status(0, 0, 0)),
                test(AND, 0, 0xFF, 0, status(0, 0, 1)),
                test(AND, 0xFF, 0, 0, status(0, 0, 1)),
                test(AND, 0xFF, 0xFF, 0xFF, status(1, 0, 0)),
                test(AND, 0xF0, 0x0F, 0, status(0, 0, 1)),
                test(AND, 0x3C, 0xC3, 0, status(0, 0, 1)),
                //SHR
                test(SHR, 1, 1, 0, status(0, 0, 1)),
                test(SHR, 2, 2, 1, status(0, 0, 0)),
                test(SHR, 4, 3, 2, status(0, 0, 0)),
                test(SHR, 0x80, 4, 0x40, status(0, 0, 0)),
                test(SHR, 0xFF, 5, 0x7F, status(0, 0, 0)),
                test(SHR, 0x0F, 6, 0x07, status(0, 0, 0)),
                test(SHR, 0xF0, 7, 0x78, status(0, 0, 0)),
                //SHL
                test(SHL, 1, 1, 2, status(0, 0, 0)),
                test(SHL, 2, 2, 4, status(0, 0, 0)),
                test(SHL, 4, 3, 8, status(0, 0, 0)),
                test(SHL, 0x80, 4, 0x00, status(0, 0, 1)),
                test(SHL, 0xFF, 5, 0xFE, status(1, 0, 0)),
                test(SHL, 0x0F, 6, 0x1E, status(0, 0, 0)),
                test(SHL, 0xF0, 7, 0xE0, status(1, 0, 0)),
                //ROR
                test(ROR, 0, 0, 0, status(0, 0, 1)),
                test(ROR, 1, 1, 0x80, status(1, 0, 0)),
                test(ROR, 2, 2, 1, status(0, 0, 0)),
                test(ROR, 4, 3, 2, status(0, 0, 0)),
                test(ROR, 0x80, 4, 0x40, status(0, 0, 0)),
                test(ROR, 0xFF, 5, 0xFF, status(1, 0, 0)),
                test(ROR, 0x0F, 6, 0x87, status(1, 0, 0)),
                test(ROR, 0xF0, 7, 0x78, status(0, 0, 0)),
                //ROL
                test(ROL, 0, 0, 0, status(0, 0, 1)),
                test(ROL, 1, 1, 2, status(0, 0, 0)),
                test(ROL, 2, 2, 4, status(0, 0, 0)),
                test(ROL, 4, 3, 8, status(0, 0, 0)),
                test(ROL, 0x80, 4, 1, status(0, 0, 0)),
                test(ROL, 0xFF, 5, 0xFF, status(1, 0, 0)),
                test(ROL, 0x0F, 6, 0x1E, status(0, 0, 0)),
                test(ROL, 0xF0, 7, 0xE1, status(1, 0, 0)),
            };
            //Test each case
            StatusRegister register = new StatusRegister(new Register(Registers.Status, 0));
            foreach(var testCase in testCases)
            {
                string id = $"{testCase.Item1}({testCase.Item2}, {testCase.Item3})";
                Assert.AreEqual(
                    testCase.Item4,
                    alu.Execute(testCase.Item1, testCase.Item2, testCase.Item3, ref register),
                    id);
                Assert.IsTrue(((byte) register.Value & testCase.Item5) == testCase.Item5, id);
            }
        }

        [TestMethod]
        public void TestDivisionByZero()
        {
            try
            {
                StatusRegister status = new StatusRegister(new Register(Registers.Status, 0));
                alu.Execute(DIV, 5, 0, ref status);
                Assert.Fail("Division by 0 doesn't throw an exception");
            }
            catch (DivideByZeroException) { }
        }
    }
}
