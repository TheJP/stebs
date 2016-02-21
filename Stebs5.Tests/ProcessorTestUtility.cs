using ProcessorSimulation;
using ProcessorSimulation.Device;
using ProcessorSimulation.MpmParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stebs5.Tests
{
    /// <summary>
    /// Class that contains one instance of a processor and provides methods for modifications, simulation and assertions.
    /// </summary>
    public class ProcessorTestUtility
    {
        public IProcessor Processor { get; }
        public RegisterFactory RegisterFactory { get; }
        public IProcessorSimulator Simulator { get; }

        public static Lazy<Mpm> Mpm = new Lazy<Mpm>(() =>
        {
            var mpm = new Mpm(new MpmFileParser());
            mpm.Parse("Resources\\INSTRUCTION.data", "Resources\\ROM1.data", "Resources\\ROM2.data");
            return mpm;
        });

        public ProcessorTestUtility()
        {
            RegisterFactory = (type, data) => new Register(type, data);
            Processor = new Processor(new Alu(RegisterFactory), new Ram(), RegisterFactory, new DeviceManager());
            Simulator = new ProcessorSimulator(Mpm.Value);
        }

        /// <summary>Alternative constructor with given initial RAM.</summary>
        /// <param name="initialRam">Initial data of the RAM.</param>
        public ProcessorTestUtility(byte[] initialRam) : this()
        {
            SetRam(initialRam);
        }

        /// <summary>
        /// Makes sure, that the returned result has the same size as required by the RAM.
        /// If the input is too short, it will be filled up with zeros. If the input is too long it will be cropped.
        /// </summary>
        private byte[] AssureLength(byte[] data)
        {
            if (data.Length != Processor.Ram.Data.Count())
            {
                //Adjust length to fit
                var tmp = new byte[Processor.Ram.Data.Count()];
                for (int i = Math.Min(tmp.Length, data.Length) - 1; i >= 0; --i)
                {
                    tmp[i] = data[i];
                }
                data = tmp;
            }
            return data;
        }

        /// <summary>Replaces the data of the RAM with the given new data.</summary>
        /// <param name="data"></param>
        public void SetRam(byte[] data)
        {
            data = AssureLength(data);
            Processor.Execute(session => session.RamSession.Set(data));
        }

        /// <summary>Set a single address of the RAM to the given value.</summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void SetRam(byte address, byte value) => Processor.Execute(session => session.RamSession.Set(address, value));

        /// <summary>Sets the register to the given value.</summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetRegister(Registers type, uint value) => Processor.Execute(session => session.SetRegister(type, value));

        /// <summary>Simulates one instruction step.</summary>
        public void SimulateInstructionStep() => Simulator.ExecuteInstructionStep(Processor);
        /// <summary>Simulates one macro step.</summary>
        public void SimulateMacroStep() => Simulator.ExecuteMacroStep(Processor);
        /// <summary>Simulates one micro step.</summary>
        public void SimulateMicroStep() => Simulator.ExecuteMicroStep(Processor);
        /// <summary>Simulates the given amount of instruction steps.</summary>
        /// <param name="times"></param>
        public void SimulateInstructionStep(int times)
        {
            for (int i = 0; i < times; ++i) { SimulateInstructionStep(); }
        }
        /// <summary>Simulates the given amount of macro steps.</summary>
        public void SimulateMacroStep(int times)
        {
            for (int i = 0; i < times; ++i) { SimulateMacroStep(); }
        }
        /// <summary>Simulates the given amount of micro steps.</summary>
        public void SimulateMicroStep(int times)
        {
            for (int i = 0; i < times; ++i) { SimulateMicroStep(); }
        }

        /// <summary>Simulates until the processor halts or until the number of stebs specified in "abort" are reached.</summary>
        /// <param name="abort"></param>
        public void SimulateUntilHalt(int? abort = null)
        {
            uint steps = 0;
            while (!Processor.IsHalted)
            {
                SimulateInstructionStep();
                ++steps;
                if (abort.HasValue && steps >= abort.Value) { Assert.Fail("Processor did not halt after the specified number of steps."); }
            }
        }

        /// <summary>Asserts, that the given expected RAM is equal to the actual RAM.</summary>
        /// <param name="expected"></param>
        public void AssertRamEquals(byte[] expected)
        {
            using (var ram = new Ram().CreateSession())
            {
                ram.Set(AssureLength(expected));
                Helper.DictionaryEqual(ram.Ram.Data, Processor.Ram.Data);
            }
        }

        /// <summary>Asserts, that the RAM contains the given expected value at the given address.</summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public void AssertRamEquals(byte address, byte value, string message = null) =>
            Assert.AreEqual(value, Processor.Ram.Data[address], message ?? $"RAM [{address}] does not contain the expected value");

        /// <summary>Asserts, that the value of the register of the given type is equal to the given value.</summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public void AssertRegisterEquals(Registers type, uint value, string message = null)
        {
            var register = Processor.Registers[type];
            Assert.AreEqual(value, register.Value, message ?? $"Register {type} does not have the expected value");
        }

        /// <summary>Asserts that the given expected register is equal to the actual register with the same type.</summary>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        public void AssertRegisterEquals(IRegister expected, string message = null)
        {
            var actual = Processor.Registers[expected.Type];
            Assert.AreEqual(expected.Value, actual.Value, message ?? $"Register {expected.Type} does not have the expected value");
        }
    }
}
