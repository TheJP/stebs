using ProcessorSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    public interface IDispatcher
    {
        /// <summary>
        /// Add a <see cref="IProcessor"/> to be managed by the <see cref="IDispatcher"/>.
        /// </summary>
        /// <param name="running">Determines if the processor is running. (Default: false)</param>
        /// <param name="runDelay">Minimum time between two automatic simulation steps. (Default: 0)</param>
        /// <param name="stepSize">Step size, which is used in automatic simulation steps. (Default: <see cref="SimulationStepSize.Instruction"/>)</param>
        /// <returns>Immutable <see cref="IDispatcherItem"/>, which encapsules the <see cref="IProcessor"/>.</returns>
        IDispatcherItem CreateProcessor(bool running = false, TimeSpan runDelay = default(TimeSpan), SimulationStepSize stepSize = SimulationStepSize.Instruction);

        /// <summary>
        /// Returns the <see cref="IDispatcherItem"/> of the <see cref="IProcessor"/> with the given <see cref="Guid"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> referencing a <see cref="IProcessor"/></param>
        /// <returns>Immutable <see cref="IDispatcherItem"/>, which encapsules the <see cref="IProcessor"/>.</returns>
        IDispatcherItem this[Guid id] { get; }

        /// <summary>
        /// Checks if the given <see cref="Guid"/> is conatined.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true = contained, false = not contained</returns>
        bool ContainsGuid(Guid id);

        /// <summary>
        /// Replaces the item with the <see cref="Guid"/> of the given <see cref="IDispatcherItem"/> with the given <see cref="IDispatcherItem"/>.
        /// </summary>
        /// <param name="updated">Updated <see cref="IDispatcherItem"/></param>
        /// <param name="comparison">Comparison <see cref="IDispatcherItem"/>: This is the previous value, which is used for the compare and swap.</param>
        /// <returns>Returns if the update was successful.</returns>
        bool Update(IDispatcherItem updated, IDispatcherItem comparison);

        /// <summary>
        /// Removes the <see cref="IProcessor"/> with the given <see cref="Guid"/> from the <see cref="IDispatcher"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> of the <see cref="IProcessor"/> which should be removed.</param>
        /// <returns>Returns if the remove was successful.</returns>
        bool Remove(Guid id);

        /// <summary>
        /// Queue a step request for the <see cref="IProcessor"/> with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stepSize"></param>
        void Step(Guid id, SimulationStepSize stepSize);

        /// <summary>
        /// Start executing steps of the managed <see cref="IProcessor"/>s.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop executing steps of the managed <see cref="IProcessor"/>s. The <see cref="IDispatcher"/> can be restarted after a stop.
        /// </summary>
        void Stop();
    }
}
