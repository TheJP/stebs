using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    /// <summary>
    /// Concrete implementation of the <see cref="IMpmFileParser"/>. See all documentation there.
    /// </summary>
    public class MpmFileParser : IMpmFileParser
    {
        /// <summary>
        /// Encoding, which is used for all file readings, which are done in this class.
        /// </summary>
        private static readonly Encoding encoding = Encoding.ASCII;

        /// <summary>
        /// Dictionary, which is used to convert the input to the correct Destination registers.
        /// </summary>
        private static readonly Dictionary<int, Destination> numberToDestination = new Dictionary<int, Destination>()
        {
            [0] = Destination.Empty,
            [1] = Destination.Y,
            [2] = Destination.X,
            [3] = Destination.SR,
            [4] = Destination.SEL_REF,
            [5] = Destination.SEL,
            [6] = Destination.RES,
            [7] = Destination.MDR,
            [8] = Destination.MBR,
            [9] = Destination.MAR,
            [10] = Destination.IR,
            [11] = Destination.IP
        };

        /// <summary>
        /// Dictionary, which is used to convert the input to the correct Source registers.
        /// </summary>
        private static readonly Dictionary<int, Source> numberToSource = new Dictionary<int, Source>()
        {
            [0] = Source.Empty,
            [3] = Source.SR,
            [4] = Source.SEL_REF,
            [6] = Source.RES,
            [7] = Source.MDR,
            [8] = Source.MBR,
            [9] = Source.MAR,
            [11] = Source.IP,
            [12] = Source.Data
        };

        private IEnumerable<IInstruction> ParseInstructions(TextReader reader)
        {
            var result = new List<IInstruction>();
            var csvConfig = new CsvConfiguration();
            csvConfig.HasHeaderRecord = false;
            csvConfig.SkipEmptyRecords = true;
            csvConfig.TrimFields = true;
            csvConfig.Delimiter = ";";
            var csv = new CsvReader(reader, csvConfig);
            while (csv.Read())
            {
                var address = int.Parse(csv.GetField<string>(0), NumberStyles.HexNumber);
                var opCode = byte.Parse(csv.GetField<string>(1), NumberStyles.HexNumber);
                var type = csv.GetField<string>(2).Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var operandString = type.Length >= 2 ? type[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                var operandTypes = OperandType.FromStrings(operandString);
                //TODO: Remove too tight coupling
                result.Add(new Instruction(opCode, address, type[0], operandTypes));
            }
            return result;
        }

        public IEnumerable<IInstruction> ParseInstructions(string instructions)
        {
            using (TextReader reader = new StringReader(instructions))
            {
                return ParseInstructions(reader);
            }
        }

        public IEnumerable<IInstruction> ParseInstructionsFile(string filename)
        {
            using(TextReader reader = new StreamReader(filename, encoding))
            {
                return ParseInstructions(reader);
            }
        }

        private IDictionary<byte, IMicroInstruction> ParseMicroInstructions(TextReader reader)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public IDictionary<byte, IMicroInstruction> ParseMicroInstructions(string microInstructions)
        {
            using(TextReader reader = new StringReader(microInstructions))
            {
                return ParseMicroInstructions(microInstructions);
            }
        }

        public IDictionary<byte, IMicroInstruction> ParseMicroInstructionsFile(string filename)
        {
            using (TextReader reader = new StreamReader(filename, encoding))
            {
                return ParseMicroInstructions(reader);
            }
        }
    }
}
