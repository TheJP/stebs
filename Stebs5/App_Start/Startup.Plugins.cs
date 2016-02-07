using PluginApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Stebs5
{
    public partial class Startup
    {
        /// <summary>
        /// Load all assemblies which are found in the plugin folder.
        /// </summary>
        private void LoadPluginAssemblies(IConstants constants)
        {
            if (!Directory.Exists(constants.PluginsAbsolutePath))
            {
                Directory.CreateDirectory(constants.PluginsAbsolutePath);
            }
            foreach(var file in Directory.EnumerateFiles(constants.PluginsAbsolutePath, "*.dll", SearchOption.AllDirectories))
            {
                try { Assembly.LoadFrom(file); }
                catch (Exception) { } //TODO: Log warning
            }
        }

        /// <summary>
        /// Registers all device plugins in the plugin manager.
        /// </summary>
        private void AddAllDevicePlugins(IPluginManager pluginManager)
        {
            try
            {
                var pluginType = typeof(IDevicePlugin);
                //Search all concrete plugin implementations
                var plugins = AppDomain.CurrentDomain.GetAssemblies()
                    .AsParallel()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => pluginType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    .ToList();
                //Register the plugins
                foreach (var plugin in plugins)
                {
                    var devicePlugin = (IDevicePlugin)Activator.CreateInstance(plugin);
                    pluginManager.Register(devicePlugin);
                }
            }
            catch (ReflectionTypeLoadException) { } //TODO: Log Warning
        }
    }
}