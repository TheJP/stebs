using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    /// <summary>
    /// IProcessorSimulator, which simualtes using given microinstructions.
    /// </summary>
    public class ProcessorSimulator : IProcessorSimulator
    {
        public ProcessorSimulator()
        {
            //TODO: take readonly microinstructions
        }

        public void ExecuteMicroStep(IProcessor processor)
        {
            //TODO: microstep
        }
    }
}
