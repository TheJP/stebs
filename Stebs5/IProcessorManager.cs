using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessorSimulation;
using ProcessorDispatcher;

namespace Stebs5
{
    /// <summary>
    /// Manages the combination between client ids and processors.
    /// It adds <see cref="IProcessor"/>s to the <see cref="IDispatcher"/> and converts client requests and transmits them to the <see cref="IDispatcher"/>.
    /// </summary>
    public interface IProcessorManager
    {
        /// <summary>
        /// Create new processor for client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>Id of the new processor.</returns>
        Guid CreateProcessor(string clientId);

        /// <summary>
        /// Creates new processor for client id, if it does not exist yet.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>Id of the processor.</returns>
        Guid AssureProcessorExists(string clientId);

        /// <summary>
        /// Removes processor if it exists for the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>Id of the removed processor.</returns>
        Guid? RemoveProcessor(string clientId);

        /// <summary>
        /// Set the ram of the processor of the given client id to the given new content.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="newContent">New content, which should be set to the ram.</param>
        void ChangeRamContent(string clientId, int[] newContent);

        /// <summary>
        /// Set the state of the processor of the given client id to running.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="stepSize">Initial step size, which is used for the simulation.</param>
        void Run(string clientId, SimulationStepSize stepSize = SimulationStepSize.Instruction);

        /// <summary>
        /// Set the state of the processor of the given client id to not running.
        /// </summary>
        /// <param name="clientId"></param>
        void Pause(string clientId);

        /// <summary>
        /// Stops the processor of the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        void Stop(string clientId);

        /// <summary>
        /// Execute a step with the given size on the processor of the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="stepSize"></param>
        void Step(string clientId, SimulationStepSize stepSize = SimulationStepSize.Instruction);

        /// <summary>
        /// Change step size of automatic running processor of the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="stepSize"></param>
        void ChangeSetpSize(string clientId, SimulationStepSize stepSize);

        /// <summary>
        /// Change minimum run delay of automatic processor of the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="runDelay"></param>
        void ChangeRunDelay(string clientId, TimeSpan runDelay);
    }
}
