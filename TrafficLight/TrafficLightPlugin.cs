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
        public string DeviceTemplate(byte slot) => $@"Data: <span id=""traffic-data-{slot}"">0</span>" +
            $@"<script>Stebs.registerDevice({slot}, function(data){{ $('#traffic-data-{slot}').text(data.Data); }});</script>";

        public string Name => "Traffic Light";

        public string PluginId => "TrafficLight";

        public IDevice CreateDevice() => new TrafficLightDevice();
    }
}
