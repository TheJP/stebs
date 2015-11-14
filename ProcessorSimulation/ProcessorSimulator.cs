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

        /// <summary>Micro programm memory address at which there is mpm code to fetch the next mpm address using the opcode decoder.</summary>
        private const uint FetchAddress = 0x000;
        /// <summary>Micro programm memory address at which an interrupt handling is executed. (=> Jump to the interrupt routine)</summary>
        private const uint InterruptAddress = 0x010;

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

                session.SetRegister(Registers.MIP, NextMip(processor, mpmEntry));
                //TODO: Implement halt
                //TODO: microstep
                processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Micro);
            }
        }

        /// <summary>
        /// Calculates the next micro instruction pointer (MIP).
        /// This is calculated from the processor state and the current micro program memory entry.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mpmEntry">Current micro program memory entry</param>
        /// <returns>New micro instruction pointer</returns>
        private uint NextMip(IProcessor processor, IMicroInstruction mpmEntry)
        {
            var mip = processor.Registers[Registers.MIP].Value;
            var status = new StatusRegister(processor.Registers[Registers.Status]);
            switch (mpmEntry.NextAddress)
            {
                case NextAddress.Next:
                    return mip + (uint)(SuccessfulJump(status, mpmEntry.JumpCriterion) ? mpmEntry.Value : 1);
                case NextAddress.Decode:
                    var instruction = processor.Registers[Registers.IR].Value;
                    return (uint)mpm.Instructions[(byte)instruction].MpmAddress;
                case NextAddress.Fetch:
                    var interruptEnabled = processor.Registers[Registers.InterruptEnabled];
                    return (status.Interrupt && interruptEnabled.Value == 1) ? InterruptAddress : FetchAddress;
                default:
                    throw new NotImplementedException();
            }
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
