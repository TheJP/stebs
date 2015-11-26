using ProcessorSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    /// <summary>
    /// Collector, which uses processor and ram events to find changes and collect them in the public dictionaries.
    /// </summary>
    public interface IChangesCollector
    {
        /// <summary>
        /// Binds the collector to the given processor. (Subscribes to its events.)
        /// </summary>
        /// <param name="processor"></param>
        void BindTo(IProcessor processor);

        /// <summary>
        /// Unbinds the collector from the previous processor. (Stops collecting data.)
        /// </summary>
        void Unbind();

        /// <summary>
        /// Changes that happened to the ram during the subscribed phase.
        /// </summary>
        IDictionary<byte, byte> RamChanges { get; }

        /// <summary>
        /// Changes that happened to the registers during the subscribed phase.
        /// </summary>
        IDictionary<Registers, IRegister> RegisterChanges { get; }
    }
}
