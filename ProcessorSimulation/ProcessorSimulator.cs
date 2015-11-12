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
        //Immutable IMpm (allowed because it's not a state)
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

                SetNextMip(session, mpmEntry);
                //TODO: microstep
                processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Micro);
            }
        }

        /// <summary>
        /// Set the next micro instruction pointer (MIP).
        /// This is calculated from the processor state and the current micro program memory entry.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mpmEntry">Current micro program memory entry</param>
        private void SetNextMip(IProcessorSession session, IMicroInstruction mpmEntry)
        {
            var newMip = session.Processor.Registers[Registers.MIP].Value;
            switch (mpmEntry.NextAddress)
            {
                case NextAddress.Next:
                    var status = new StatusRegister(session.Processor.Registers[Registers.Status]);
                    newMip += (uint)(SuccessfulJump(status, mpmEntry.JumpCriterion) ? mpmEntry.Value : 1);
                    break;
                case NextAddress.Decode:
                    var instruction = session.Processor.Registers[Registers.IR].Value;
                    newMip = mpm.Instructions[(byte)instruction].OpCode;
                    break;
                    //TODO: Other case
            }
            session.SetRegister(Registers.MIP, newMip);
        }

        /// <summary>Checks if the micro program memory entry is a jump and it is successful.</summary>
        /// <param name="status">Status register</param>
        /// <param name="criterion">Jump criterion of the current micro program memory entry</param>
        /// <returns></returns>
        private bool SuccessfulJump(StatusRegister status, JumpCriterion criterion) =>
            criterion != JumpCriterion.Empty && (
                (criterion == JumpCriterion.Signed && status.Signed) ||
                (criterion == JumpCriterion.Overflow && status.Overflow) ||
                (criterion == JumpCriterion.Zero && status.Zero) ||
                (criterion == JumpCriterion.NotSigned && !status.Signed) ||
                (criterion == JumpCriterion.NoOverflow && !status.Overflow) ||
                (criterion == JumpCriterion.NotZero && !status.Zero)
            );
    }
}
