using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Parser, which takes raw configuration strings and extract information for the mpm (micro program memory) and the opcode decoder.
    /// </summary>
    public interface IMpmParser
    {
        /// <summary>
        /// Decodes the given raw instructions.
        /// </summary>
        /// <param name="instructions">All instructions given in a csv format. The csv format should not contain headers.</param>
        /// <returns>Parsed instructions.</returns>
        IEnumerable<IInstruction> ParseInstructions(string instructions);

        /// <summary>
        /// Decodes the given micro instructions.
        /// </summary>
        /// <param name="microInstructions">
        /// Micro instructions in a logisim ram export file format.
        /// (This format has to be used beacause of compatibility reasons.)
        /// </param>
        /// <returns>Parsed micro instructions in a dictionary. The key is the micro instructions address.</returns>
        IDictionary<byte, IMicroInstruction> ParseMicroInstructions(string microInstructions);
    }
}
