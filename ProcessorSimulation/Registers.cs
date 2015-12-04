using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    /// <summary>Extension methods for the registers enum.</summary>
    public static class RegistersExtensions
    {
        public static Registers[] GetValues() => (Registers[])Enum.GetValues(typeof(Registers));
    }

    public enum Registers
    {
        /// <summary>
        /// Data register A.
        /// One of the four registers, that are meant for temporary storage of data.
        /// </summary>
        AL = 0,

        /// <summary>
        /// Data register B.
        /// One of the four registers, that are meant for temporary storage of data.
        /// </summary>
        BL = 1,

        /// <summary>
        /// Data register C.
        /// One of the four registers, that are meant for temporary storage of data.
        /// </summary>
        CL = 2,

        /// <summary>
        /// Data register D.
        /// One of the four registers, that are meant for temporary storage of data.
        /// </summary>
        DL = 3,

        /// <summary>
        /// Stack pointer. This register points to the next free stack address.
        /// </summary>
        SP = 4,

        /// <summary>
        /// Selection of the data registers.
        /// This register is used to determine which data register to use, if a one is accessed.
        /// </summary>
        SEL = 5,

        /// <summary>
        /// Instruction pointer.
        /// Register that points to the memory address where the current instruction, one of its parameter or the next instruction is stored.
        /// </summary>
        IP = 6,

        /// <summary>
        /// Memory buffer register.
        /// Register that is used by the processor to buffer data, which is only relevant during one instruction.
        /// </summary>
        MBR = 7,

        /// <summary>
        /// Memory address register.
        /// Register which describes the data, which is currently on the address bus.
        /// </summary>
        MAR = 9,

        /// <summary>
        /// Memory data register.
        /// Register which describes the data, which is currently on the data bus.
        /// </summary>
        MDR = 10,

        /// <summary>
        /// Instruction register.
        /// Register that stores the currently executed instruction.
        /// </summary>
        IR = 11,

        /// <summary>
        /// ALU register X.
        /// Left hand side parameter of ALU commands.
        /// </summary>
        X = 17,

        /// <summary>
        /// ALU register Y.
        /// Right hand side parameter of ALU commands.
        /// </summary>
        Y = 18,

        /// <summary>
        /// ALU result register.
        /// Register that stores the result of the last ALU command.
        /// </summary>
        RES = 12,

        /// <summary>
        /// Micro instruction pointer.
        /// Register that stores the micro program address of the current executed micro instruction.
        /// Caution: This is a 12 bit register.
        /// </summary>
        MIP = 19,

        /// <summary>
        /// Status Register which containes flags.
        /// The contained flags are:
        /// I=Interrupt Flag that determines that an interrupt has to be handled.
        /// This flag is changed with the instructions CLI (set to 0) and STI (set to 1).
        /// If the flag is 1, incoming interrupts are handled.
        /// S=Signed Alu flag that is set, if the last alu instruction had a signed result.
        /// O=Overflow Alu flag that is set, if there was an overflow in the last alu instruction.
        /// Z=Zero Alu flag that is set, if the last alu instruction had zero as result.
        /// </summary>
        Status = 20,

        /// <summary>
        /// Register which contains the IRF (Interrupt Flag) at the least significant position.
        /// This flag determines, if there is a pending interrupt (flag = 1), which has to be handled or not (flag = 0).
        /// </summary>
        Interrupt = 21
    }
}
