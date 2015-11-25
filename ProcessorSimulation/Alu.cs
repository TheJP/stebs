using System;
using System.Collections.Generic;
using static ProcessorSimulation.AluCmd;

namespace ProcessorSimulation
{
    /// <summary>
    /// Simulation of the ALU (arithmetic logic unit).
    /// </summary>
    /// The ALU is the calculating part of a CPU. It can execute mathematical 
    /// operations such adding two values or increment one value. It has
    /// two registers (X and Y) which are required to execute binary or
    /// unairy commands. 
    public class Alu : IAlu
    {
        /// <summary>
        /// Dictonary which contains every command that can be computed by the Alu.
        /// </summary>
        private Dictionary<AluCmd, Func<byte, byte, int>> commands = new Dictionary<AluCmd, Func<byte, byte, int>>()
        {
            [NOP] = (x, y) => 0,
            [ADD] = (x, y) => x + y,
            [SUB] = (x, y) => x - y,
            [MUL] = (x, y) => x * y,
            [DIV] = (x, y) => x / y,
            [MOD] = (x, y) => x % y,
            [DEC] = (x, y) => x - 1,
            [INC] = (x, y) => x + 1,
            [ OR] = (x, y) => x | y,
            [XOR] = (x, y) => x ^ y,
            [NOT] = (x, y) => ~x,
            [AND] = (x, y) => x & y,
            [SHR] = (x, y) => unchecked(x >> 1),
            [SHL] = (x, y) => unchecked(x << 1),
            [ROR] = (x, y) => unchecked(x >> 1) | ((x & 0x01) << 7),
            [ROL] = (x, y) => unchecked(x << 1) | ((x & 0x80) >> 7)
        };

        private readonly Func<Registers, uint, IRegister> registerFactory;

        public Alu(Func<Registers, uint, IRegister> registerFactory)
        {
            this.registerFactory = registerFactory;
        }

        public byte Execute(AluCmd command, byte x, byte y, ref StatusRegister status)
        {
            byte result = 0;
            try
            {
                result = checked((byte)commands[command](x, y));
                status = status.SetOverflow(false, registerFactory);
            }
            catch (OverflowException)
            {
                result = unchecked((byte)commands[command](x, y));
                status = status.SetOverflow(true, registerFactory);
            }
            status = status.SetSigned((result & 0x80) != 0, registerFactory);
            status = status.SetZero(result == 0, registerFactory);
            return result;
        }
    }
}
