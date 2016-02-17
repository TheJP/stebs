using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterruptDevice
{
    public class ChangeIntervalCommand : NetworkCommand
    {
        public int NewInterval { get; set; }
    }
}
