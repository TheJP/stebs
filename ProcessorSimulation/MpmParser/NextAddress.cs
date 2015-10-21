using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Enumeration which describes how to calculate the next micro instruction address.
    /// </summary>
    public enum NextAddress
    {
        Fetch = 1,
        Decode = 2,
        Next = 3
    }
}
