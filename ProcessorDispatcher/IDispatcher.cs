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
        KeyValuePair<Guid, IProcessor>? CreateProcessor(bool running = false, TimeSpan runDelay = default(TimeSpan), SimulationStepSize stepSize = SimulationStepSize.Instruction);
        void Start();
        void Step(Guid id, SimulationStepSize stepSize);
    }
}
