using ProcessorSimulation.MpmParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stebs5.Models
{
    public class InitialiseViewModel
    {
        public IDictionary<byte, IInstruction> Instructions { get; }
        public IEnumerable<string> Registers { get; }
        public IDictionary<string, DeviceViewModel> DeviceTypes { get; }
        public string ProcessorId { get; }
        public InitialiseViewModel(
            IDictionary<byte, IInstruction> instructions, IEnumerable<string> registers,
            IDictionary<string, DeviceViewModel> deviceTypes, string processorId)
        {
            this.Instructions = instructions;
            this.Registers = registers;
            this.DeviceTypes = deviceTypes;
            this.ProcessorId = processorId;
        }
    }
}