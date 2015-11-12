using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// See documentation in <see cref="IMpm"/>.
    /// </summary>
    public class Mpm : IMpm
    {
        public IDictionary<byte, IInstruction> Instructions { get; private set; }

        public IDictionary<int, IMicroInstruction> MicroInstructions { get; private set; }

        public string RawInstructions { get; private set; }

        /// <summary>
        /// Parser, which is used to parse the mpm information which is stored in this object.
        /// </summary>
        private readonly IMpmFileParser parser;

        /// <summary>
        /// Encoding, which is used for all file readings, which are done in this class.
        /// </summary>
        private static readonly Encoding encoding = Encoding.ASCII;

        public Mpm(IMpmFileParser parser)
        {
            this.parser = parser;
        }

        public void Parse(string instructionsFilename, string rom1Filename, string rom2Filename)
        {
            using(TextReader reader = new StreamReader(instructionsFilename, encoding))
            {
                RawInstructions = reader.ReadToEnd();
            }
            Instructions = parser.ParseInstructions(RawInstructions);
            MicroInstructions = parser.ParseMicroInstructionsFile(rom1Filename, rom2Filename).ToImmutableDictionary();
        }
    }
}
