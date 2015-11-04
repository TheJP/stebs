using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5
{
    /// <summary>
    /// Contains constants, which are not class specific, but are used in multiple locations in the application.
    /// </summary>
    public interface IConstants
    {
        /// <summary>
        /// Absolute path to the instructions data file. This file is used for the processor simulation.
        /// </summary>
        string InstructionsAbsolutePath { get; }
    }
}
