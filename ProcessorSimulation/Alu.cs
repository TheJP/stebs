using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Alu
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
            [SHR] = (x, y) => x >> 1,
            [SHL] = (x, y) => x << 1,
            [ROR] = (x, y) => unchecked(x >> 1) | ((x & 0x01) << 7),
            [ROL] = (x, y) => unchecked(x << 1) | ((x & 0x80) >> 7)
        };

        public byte Execute(AluCmd command, byte x, byte y)
        {
            byte result = 0;
            Action cmd = () => result = (byte)commands[command](x, y);
            try
            {
                checked { cmd(); }
            }
            catch (OverflowException)
            {
                unchecked { cmd(); }
            }
            return result;
        }
    }
}
