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

        /// <summary>Replaces the data of the RAM with the given new data.</summary>
        /// <param name="data"></param>
        public void SetRam(byte[] data)
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

        public void SimulateInstructionStep() => Simulator.ExecuteInstructionStep(Processor);
        public void SimulateMacroStep() => Simulator.ExecuteMacroStep(Processor);
        public void SimulateMicroStep() => Simulator.ExecuteMicroStep(Processor);
        public void SimulateInstructionStep(int times)
        {
            for (int i = 0; i < times; ++i) { SimulateInstructionStep(); }
        }
        public void SimulateMacroStep(int times)
        {
            for (int i = 0; i < times; ++i) { SimulateMacroStep(); }
        }
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
                if (abort.HasValue && steps >= abort) { Assert.Fail("Processor did not halt after the specified number of steps."); }
            }
        }

        /// <summary>Asserts, that the given expected RAM is equal to the actual RAM.</summary>
        /// <param name="expected"></param>
        public void AssertRamEquals(byte[] expected)
        {
            using (var ram = new Ram().CreateSession())
            {
                ram.Set(expected);
                Helper.DictionaryEqual(ram.Ram.Data, Processor.Ram.Data);
            }
        }

        /// <summary>Asserts, that the value of the register of the given type is equal to the given value.</summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void AssertRegisterEquals(Registers type, uint value)
        {
            var register = Processor.Registers[type];
            Assert.IsTrue(value == register.Value, $"Register is {register.Value} but should be {value}.");
        }

        /// <summary>Asserts that the given expected register is equal to the actual register with the same type.</summary>
        /// <param name="expected"></param>
        public void AssertRegisterEquals(IRegister expected)
        {
            var actual = Processor.Registers[expected.Type];
            Assert.IsTrue(actual.Value == expected.Value, $"Register is {actual.Value} but should be {expected.Value}.");
        }
    }
}
