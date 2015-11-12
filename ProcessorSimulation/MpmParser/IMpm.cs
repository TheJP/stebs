using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{

    /// <summary>
    /// Micro program memory. Instances of this class opcode decoder and mpm information.
    /// </summary>
    public interface IMpm
    {
        /// <summary>
        /// Raw instructions configuration file.
        /// The instructions of the <see cref="Instructions"/> property are generated from this input.
        /// </summary>
        string RawInstructions { get; }

        /// <summary>
        /// The instructions, in the format they are needed for the opcode decoder.
        /// </summary>
        IDictionary<byte, IInstruction> Instructions { get; }

        /// <summary>
        /// Containes all micro instructions, referenced by their mpm address.
        /// Mutative access to this dictionary is not possible.
        /// </summary>
        IDictionary<int, IMicroInstruction> MicroInstructions { get; }

        /// <summary>
        /// This Method provides the mpm with initial data from the three given files.
        /// </summary>
        /// <param name="instructionsFilename"></param>
        /// <param name="rom1Filename"></param>
        /// <param name="rom2Filename"></param>
        void Parse(string instructionsFilename, string rom1Filename, string rom2Filename);
    }
}
