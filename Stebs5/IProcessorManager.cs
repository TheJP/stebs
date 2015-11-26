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
        void CreateProcessor(string clientId);

        /// <summary>
        /// Creates new processor for client id, if it does not exist yet.
        /// </summary>
        /// <param name="clientId"></param>
        void AssureProcessorExists(string clientId);

        /// <summary>
        /// Removes processor if it exists for the given client id.
        /// </summary>
        /// <param name="clientId"></param>
        void RemoveProcessor(string clientId);

        /// <summary>
        /// Set the ram of the processor of the given client id to the given new content.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="newContent">New content, which should be set to the ram.</param>
        void ChangeRamContent(string clientId, int[] newContent);
    }
}
