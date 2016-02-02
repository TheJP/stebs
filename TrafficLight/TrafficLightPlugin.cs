using PluginApi;
using ProcessorSimulation.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficLightPlugin : IDevicePlugin
    {
        public string DeviceTemplate => "Data: <span class=\"traffic-data\">0</span>";

        public string Name => "Traffic Light";

        public string PluginId => "TrafficLight";

        public IDevice CreateDevice() => new TrafficLightDevice();
    }
}
