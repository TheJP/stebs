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
        /// <summary>
        /// Absolute path to the rom1 data file. This file is used for the processor simulation.
        /// </summary>
        string Rom1AbsolutePath { get; }
        /// <summary>
        /// Absolute path to the rom2 data file. This file is used for the processor simulation.
        /// </summary>
        string Rom2AbsolutePath { get; }
        /// <summary>
        /// Absolute path to the plugin folder. This folder contains all plugin dlls which have to be loaded on startup.
        /// </summary>
        string PluginsAbsolutePath { get; }
        /// <summary>
        /// Minimal run delay, which can be set for a processor by a user.
        /// </summary>
        TimeSpan MinimalRunDelay { get; }
        /// <summary>
        /// Default run delay, with which a processor is initialized.
        /// </summary>
        TimeSpan DefaultRunDelay { get; }
    }
}
