using ProcessorSimulation;
using ProcessorSimulation.Device;
using ProcessorSimulation.MpmParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ProcessorTestUtility()
        {
            RegisterFactory = (type, data) => new Register(type, data);
            Processor = new Processor(new Alu(RegisterFactory), new Ram(), RegisterFactory, new DeviceManager());
            Simulator = new ProcessorSimulator(new Mpm(new MpmFileParser()));
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


    }
}
