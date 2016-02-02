using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stebs5.Models
{
    public class DeviceViewModel
    {
        public string Name { get; }
        public string Id { get; }
        public DeviceViewModel(string name, string id) { this.Name = name; this.Id = id; }
    }
    public class AddedDeviceViewModel
    {
        public byte Slot { get; }
        public string Template { get; }
        public bool Success { get; } = true;
        public AddedDeviceViewModel(byte slot, string template) { this.Slot = slot; this.Template = template; }
        public AddedDeviceViewModel(bool success, byte slot = 0, string template = "") : this(slot, template) { this.Success = success; }
    }
}