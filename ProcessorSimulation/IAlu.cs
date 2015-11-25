using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IAlu
    {
        /// <summary>
        /// Executes an alu calculation.
        /// </summary>
        /// <param name="command">Defines, which alu commond to execute.</param>
        /// <param name="x">Left hand side parameter.</param>
        /// <param name="y">Right hand side parameter.</param>
        /// <param name="status">Resulting status register.</param>
        /// <returns>Result of the calculation.</returns>
        byte Execute(AluCmd command, byte x, byte y, ref StatusRegister status);
    }
}
