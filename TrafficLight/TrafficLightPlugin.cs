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
        public string DeviceTemplate => @"Data: <span class=""traffic-data"">0</span>" +
            @"<script>alert('hi'); Stebs.registerDevice(1, function(data){ $('.traffic-data').text(data.Data); });</script>";

        public string Name => "Traffic Light";

        public string PluginId => "TrafficLight";

        public IDevice CreateDevice() => new TrafficLightDevice();
    }
}
