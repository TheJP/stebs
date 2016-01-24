using ProcessorSimulation.Device;

namespace Stebs5
{
    /// <summary>
    /// A device plugin is a stebs plugin, which adds devices.
    /// Devices are sensors or actors for processors. (See <see cref="IDevice" />)
    /// </summary>
    public interface IDevicePlugin
    {
        string PluginId { get; }
        void CreateDevice();
    }
}