using ProcessorSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorDispatcher
{
    /// <summary>
    /// Manages the simulation of processor steps.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Event, which is fired, when the <see cref="IDispatcher"/> finished the execution of a step simulation.
        /// 1st parameter: Processor which was simulated.
        /// 2nd parameter: Simulated step size.
        /// 3rd parameter: Changes done to the ram during the simulation step.
        /// 4th parameter: Changes done to the registers during the simulation step.
        /// </summary>
        event Action<IDispatcherItem, SimulationStepSize, IDictionary<byte, byte>, IDictionary<Registers, IRegister>> FinishedStep;

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
        /// Replaces the item with the given <see cref="Guid"/> with the <see cref="IDispatcherItem"/> resulting by applying the function.
        /// </summary>
        /// <param name="id">Id of the <see cref="IDispatcherItem"/> which should be updated.</param>
        /// <param name="updated">Pure update function. This is potentially called multiple times to guarantee thread safety.</param>
        /// <returns>Returns if the update was successful.</returns>
        bool Update(Guid id, Func<IDispatcherItem, IDispatcherItem> update);

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
