using ProcessorSimulation.MpmParser;
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
    /// <remarks>
    /// This class is not allowed to have own state, because the same
    /// instance can be used to simulate multiple processors at once.
    /// </remarks>
    public class ProcessorSimulator : IProcessorSimulator
    {
        private readonly IMpm mpm;

        public ProcessorSimulator(IMpm mpm)
        {
            this.mpm = mpm;
        }

        public void ExecuteMicroStep(IProcessor processor)
        {
            using (var session = processor.createSession())
            {
                processor.NotifySimulationStateChanged(SimulationState.Started, SimulationStepSize.Micro);
                var mpmEntry = mpm.MicroInstructions[(int)processor.Registers[Registers.MIP].Value];
                //TODO: microstep
                processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Micro);
            }
        }
    }
}
