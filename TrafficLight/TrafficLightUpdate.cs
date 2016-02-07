using ProcessorSimulation.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficLightUpdate : IDeviceUpdate
    {
        public byte Data { get; }
        public TrafficLightUpdate(byte data) { this.Data = data; }
    }
}
