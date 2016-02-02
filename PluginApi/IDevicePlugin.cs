using ProcessorSimulation.Device;

namespace PluginApi
{
    /// <summary>
    /// A device plugin is a stebs plugin, which adds devices.
    /// Devices are sensors or actors for processors. (See <see cref="IDevice" />)
    /// </summary>
    public interface IDevicePlugin
    {
        /// <summary>
        /// Name of the plugin, which is displayed to the user.
        /// </summary>
        string Name { get; }
        string PluginId { get; }
        /// <summary>
        /// Html template of the device.
        /// </summary>
        /// <param name="slot">Slot for which the template should be generated.</param>
        string DeviceTemplate(byte slot);
        IDevice CreateDevice();
    }
}