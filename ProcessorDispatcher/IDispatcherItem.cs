using ProcessorSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    /// <summary>
    /// Delegate, which defines a signature for the creation of <see cref="IDispatcherItem"/>s.
    /// </summary>
    /// <param name="guid">In memory id of the processor.</param>
    /// <param name="processor"></param>
    /// <param name="running">Determines if steps should be executed automatically.</param>
    /// <param name="runDelay">Minimum delay between steps.</param>
    /// <returns></returns>
    public delegate IDispatcherItem DispatcherItemFactory(Guid guid, IProcessor processor, bool running, TimeSpan runDelay, SimulationStepSize stepSize);

    /// <summary>
    /// Item, which is used by the <see cref="IDispatcher"/> to store references to <see cref="IProcessor"/>, <see cref="System.Guid"/> and status.
    /// </summary>
    public interface IDispatcherItem
    {
        /// <summary>In memory id of this processor.</summary>
        Guid Guid { get; }
        IProcessor Processor { get; }
        /// <summary>Determines, if steps should be simulated automatically or not.</summary>
        bool Running { get; }
        /// <summary>The minimum delay between executing a step on this dispatcher item.</summary>
        TimeSpan RunDelay { get; }
        SimulationStepSize StepSize { get; }
        /// <summary>
        /// Creates a new <see cref="IDispatcherItem"/> with changed <see cref="Running"/> attribute.
        /// </summary>
        /// <param name="value">New value for the attribute.</param>
        /// <returns></returns>
        IDispatcherItem SetRunning(bool value);
        /// <summary>
        /// Creates a new <see cref="IDispatcherItem"/> with changed <see cref="RunDelay"/> attribute.
        /// </summary>
        /// <param name="runDelay">New value for the attribute.</param>
        /// <returns></returns>
        IDispatcherItem SetRunDelay(TimeSpan runDelay);
        /// <summary>
        /// Creates a new <see cref="IDispatcherItem"/> with changed <see cref="StepSize"/> attribute.
        /// </summary>
        /// <param name="runDelay">New value for the attribute.</param>
        /// <returns></returns>
        IDispatcherItem SetStepSize(SimulationStepSize stepSize);
    }
}
