using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessorSimulation;

namespace Stebs5.Tests.ProcessorSimulation
{
    [TestClass]
    public class ProcessorSimulatorTest
    {
        private ProcessorTestUtility utility;

        [TestInitialize]
        public void Setup()
        {
            utility = new ProcessorTestUtility();
        }

        [TestMethod]
        public void TestSimpleInstructionSteps()
        {
            //MOV AL, 50
            //MOV BL, 60
            //ADD AL, BL
            //END
            utility.SetRam(new byte[] { 0xd0, 0x00, 0x50, 0xd0, 0x01, 0x60, 0xa0, 0x00, 0x01, 0x00 });
            utility.SimulateInstructionStep();
            utility.AssertRegisterEquals(Registers.AL, 0x50);
            utility.AssertRegisterEquals(Registers.BL, 0x00);
            utility.SimulateInstructionStep();
            utility.AssertRegisterEquals(Registers.AL, 0x50);
            utility.AssertRegisterEquals(Registers.BL, 0x60);
            utility.SimulateInstructionStep();
            utility.AssertRegisterEquals(Registers.AL, 0xb0);
            utility.AssertRegisterEquals(Registers.BL, 0x60);
            utility.SimulateInstructionStep();
            Assert.IsTrue(utility.Processor.IsHalted, "Processor didn't halt at the end of execution.");
        }
    }
}
