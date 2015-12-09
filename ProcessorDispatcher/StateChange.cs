using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    public enum StateChange
    {
        SoftReset = 1,
        HardReset = 2,
        Halt = 3
    }
}
