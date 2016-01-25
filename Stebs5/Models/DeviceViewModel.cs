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
}