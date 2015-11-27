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

        /// <summary>Conversion dictionary, which takes the value of the SEL register and returns the referenced register.</summary>
        private readonly Dictionary<byte, Registers> SELReference = new Dictionary<byte, Registers>()
        {
            [0x00] = Registers.AL,
            [0x01] = Registers.BL,
            [0x02] = Registers.CL,
            [0x03] = Registers.DL,
            [0x04] = Registers.SP
        };

        public ProcessorSimulator(IMpm mpm)
        {
            this.mpm = mpm;
        }

        public void ExecuteInstructionStep(IProcessor processor)
        {
            using (var session = processor.CreateSession())
            {
                processor.NotifySimulationStateChanged(SimulationState.Started, SimulationStepSize.Instruction);
                do
                {
                    ExecuteMicroStep(processor, session);
                } while (processor.Registers[Registers.MIP].Value != 0);
                processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Instruction);
            }
        }

        public void ExecuteMacroStep(IProcessor processor)
        {
            using (var session = processor.CreateSession())
            {
                processor.NotifySimulationStateChanged(SimulationState.Started, SimulationStepSize.Macro);
                do
                {
                    ExecuteMicroStep(processor, session);
                } while (processor.Registers[Registers.MIP].Value % 8 != 0);
                processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Macro);
            }
        }

        public void ExecuteMicroStep(IProcessor processor)
        {
            using (var session = processor.CreateSession())
            {
                ExecuteMicroStep(processor, session);
            }
        }

        private void ExecuteMicroStep(IProcessor processor, IProcessorSession session)
        {
            processor.NotifySimulationStateChanged(SimulationState.Started, SimulationStepSize.Micro);
            var mpmEntry = mpm.MicroInstructions[(int)processor.Registers[Registers.MIP].Value];
            //Transfer data from source to target
            session.SetRegister(Registers.MIP, NextMip(processor, mpmEntry));
            var dataBus = mpmEntry.EnableValue ? (uint)mpmEntry.Value : GetDataBusValue(processor, mpmEntry);
            WriteDataBusToRegister(session, mpmEntry, dataBus);
            //Reset interrupt flag
            if (mpmEntry.ClearInterrupt)
            {
                session.SetRegister(Registers.Interrupt, 0);
            }
            //Execute ALU command if one is contained in the mpm entry
            if (mpmEntry.AluCommand != AluCmd.NOP)
            {
                var status = new StatusRegister(processor.Registers[Registers.Status]);
                var result = processor.Alu.Execute(mpmEntry.AluCommand, (byte)processor.Registers[Registers.X].Value, (byte)processor.Registers[Registers.Y].Value, ref status);
                session.SetRegister(Registers.RES, result);
                session.SetRegister(status.Register);
            }
            //TODO: Implement halt
            processor.NotifySimulationStateChanged(SimulationState.Stopped, SimulationStepSize.Micro);
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
                    var interrupt = processor.Registers[Registers.Interrupt];
                    return (status.Interrupt && interrupt.Value == 1) ? InterruptAddress : FetchAddress;
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

        /// <summary>
        /// Returns the value, which is currently active on the databus.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="mpmEntry">Current micro program memory entry</param>
        /// <returns>DataBus value</returns>
        private uint GetDataBusValue(IProcessor processor, IMicroInstruction mpmEntry)
        {
            switch (mpmEntry.Source)
            {
                case Source.Empty:
                    return 0;
                case Source.Data:
                    //TODO: Implement IO
                    if (mpmEntry.DataInput == DataInput.IO) { throw new NotImplementedException(); }
                    var memoryAddress = (byte)processor.Registers[Registers.MAR].Value;
                    return processor.Ram.Data[memoryAddress];
                case Source.SELReferenced:
                    return processor.Registers[GetSELReferenced(processor)].Value;
                default:
                    return processor.Registers[(Registers)mpmEntry.Source].Value;
            }
        }

        /// <summary>
        /// Writes the given databus value to the register determined by the micro program memory entry.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mpmEntry">Current micro program memory entry</param>
        /// <param name="dataBus">DataBus value</param>
        private void WriteDataBusToRegister(IProcessorSession session, IMicroInstruction mpmEntry, uint dataBus)
        {
            Registers target;
            switch (mpmEntry.Destination)
            {
                case Destination.Empty:
                    return;
                case Destination.SELReferenced:
                    target = GetSELReferenced(session.Processor);
                    break;
                case Destination.MDR:
                    if (mpmEntry.ReadWrite == ReadWrite.Write)
                    {
                        //TODO: Implement IO
                        if (mpmEntry.DataInput == DataInput.IO) { throw new NotImplementedException(); }
                        var memoryAddress = (byte)session.Processor.Registers[Registers.MAR].Value;
                        session.RamSession.Set(memoryAddress, (byte)dataBus);
                    }
                    target = Registers.MDR;
                    break;
                default:
                    target = (Registers)mpmEntry.Destination;
                    break;
            }
            session.SetRegister(target, dataBus);
        }

        /// <summary>
        /// Returns the working register, which is referenced by the SEL register.
        /// </summary>
        /// <param name="processor"></param>
        /// <returns>Working register</returns>
        private Registers GetSELReferenced(IProcessor processor)
        {
            var sel = (byte)processor.Registers[Registers.SEL].Value;
            //TODO: Halt by runtime error 'invalid sel'
            if (!SELReference.ContainsKey(sel)) { throw new NotImplementedException(); }
            return SELReference[sel];
        }
    }
}
