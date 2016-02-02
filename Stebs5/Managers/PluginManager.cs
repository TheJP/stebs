using PluginApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Stebs5
{
    public class PluginManager : IPluginManager
    {
        private readonly Dictionary<string, IDevicePlugin> devicePlugins = new Dictionary<string, IDevicePlugin>();
        public IReadOnlyDictionary<string, IDevicePlugin> DevicePlugins => new ReadOnlyDictionary<string, IDevicePlugin>(devicePlugins);

        public void Register(IDevicePlugin devicePlugin)
        {
            if (devicePlugins.ContainsKey(devicePlugin.PluginId))
            {
                throw new ArgumentException("Failed to register plugin, because there was already a plugin registered with the same id.");
            }
            devicePlugins[devicePlugin.PluginId] = devicePlugin;
        }
    }
}