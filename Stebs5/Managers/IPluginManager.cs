using PluginApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5
{
    /// <summary>
    /// The plugin manager is the central registry for all plugins which are active in stebs.
    /// </summary>
    public interface IPluginManager
    {
        IReadOnlyDictionary<string, IDevicePlugin> DevicePlugins { get; }
        void Register(IDevicePlugin devicePlugin);
    }
}
