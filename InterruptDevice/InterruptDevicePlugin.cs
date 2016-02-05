using PluginApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessorSimulation.Device;

namespace InterruptDevice
{
    public class InterruptDevicePlugin : IDevicePlugin
    {
        public string Name => "Interrupt Device";

        public string PluginId => "InterruptDevice";

        public IDevice CreateDevice() => new InterruptDevice();

        public string DeviceTemplate(byte slot) =>

$@"";

    }
}
