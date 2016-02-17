using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.Device
{
    /// <summary>
    /// Represents the view of a device.
    /// The device can send data updates to the view. Mostly this data is meant to be shown to the user.
    /// </summary>
    public interface IDeviceView
    {
        void UpdateView(IDeviceUpdate update);
    }
}
