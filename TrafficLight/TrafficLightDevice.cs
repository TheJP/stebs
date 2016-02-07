using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessorSimulation.Device;

namespace TrafficLight
{
    public class TrafficLightDevice : DefaultDevice
    {
        private byte data = 0;

        public override void Input(byte input)
        {
            data = input;
            View.UpdateView(new TrafficLightUpdate(input));
        }
        public override byte Output() => data;

        public override void Reset() => Input(0xff);
    }
}
