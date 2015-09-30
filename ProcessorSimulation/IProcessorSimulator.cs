using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    /// <summary>
    /// An instance of IProcessorSimulator contains all processor simulation logic.
    /// It has to be state-less, so the same IProcessorSimulator instance can simulate multiple processors at once.
    /// </summary>
    public interface IProcessorSimulator
    {
        /// <summary>
        /// Executes a microstep on the given processor.
        /// </summary>
        /// <param name="processor"></param>
        void ExecuteMicroStep(IProcessor processor);
    }
}
