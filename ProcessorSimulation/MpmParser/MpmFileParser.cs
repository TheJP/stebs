using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

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
            [3] = Destination.Status,
            [4] = Destination.SELReferenced,
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
            [3] = Source.Status,
            [4] = Source.SELReferenced,
            [6] = Source.RES,
            [7] = Source.MDR,
            [8] = Source.MBR,
            [9] = Source.MAR,
            [11] = Source.IP,
            [12] = Source.Data
        };

        private IDictionary<byte, IInstruction> ParseInstructions(TextReader reader)
        {
            var result = ImmutableDictionary.CreateBuilder<byte, IInstruction>();
            //Csv configuration: No header, no empy lines,
            //remove leading and trailing whitespaces from cell values, ';' separated columns
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
                result.Add(opCode, new Instruction(opCode, address, type[0], operandTypes));
            }
            return result.ToImmutable();
        }

        public IDictionary<byte, IInstruction> ParseInstructions(string instructions)
        {
            using (TextReader reader = new StringReader(instructions))
            {
                return ParseInstructions(reader);
            }
        }

        public IDictionary<byte, IInstruction> ParseInstructionsFile(string filename)
        {
            using(TextReader reader = new StreamReader(filename, encoding))
            {
                return ParseInstructions(reader);
            }
        }

        private IDictionary<int, IMicroInstruction> ParseMicroInstructions(TextReader reader1, TextReader reader2)
        {
            //Skip header
            reader1.ReadLine();
            //Read an intermediate state of the micro instructions from the first reader
            var intermediates = reader1.ReadToEnd().Split()
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => int.Parse(word, NumberStyles.HexNumber))
                .Select(microcode => new
                {
                    Address = (microcode >> 20) & 0xFFF,
                    NextAddress = (NextAddress)((microcode >> 18) & 0x3),
                    ClearInterrupt = ((microcode >> 17) & 0x1) == 1,
                    EnableValue = ((microcode >> 16) & 0x1) == 1,
                    Value = (microcode >> 4) & 0xFFF,
                    Affected = ((microcode >> 3) & 1) == 1,
                    Criterion = (JumpCriterion)((microcode >> 0) & 7)
                })
                .ToDictionary(microinstruction => microinstruction.Address);
            //Skip header
            reader2.ReadLine();
            //Complete the micro instruction using the scond reader
            return reader2.ReadToEnd().Split()
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => int.Parse(word, NumberStyles.HexNumber))
                .Select(microcode =>
                {
                    var address = (microcode >> 16) & 0xFFF;
                    var entry = intermediates[address];
                    var aluCommand = (AluCmd)((microcode >> 12) & 0x0F);
                    var destination = ParseWithDictionary(numberToDestination, (microcode >> 8) & 0x0F);
                    var source = ParseWithDictionary(numberToSource, (microcode >> 4) & 0x0F);
                    var readWrite = (ReadWrite)((microcode >> 3) & 0x01);
                    var dataInput = (DataInput)((microcode >> 2) & 0x01);
                    //TODO: Remove too tight coupling
                    return (IMicroInstruction) new MicroInstruction(address, entry.NextAddress, entry.EnableValue,
                        entry.Value, entry.Criterion, entry.ClearInterrupt, entry.Affected,
                        aluCommand, source, destination, dataInput, readWrite);
                }).ToImmutableDictionary(microinstruction => microinstruction.Address);
        }

        /// <summary>
        /// Parse the given value using the given dictionary.
        /// If the value does not exist in the dictionary: Throw an MpmParsingException
        /// </summary>
        private T ParseWithDictionary<T>(IDictionary<int, T> dictionary, int value)
        {
            try
            {
                return dictionary[value];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new MpmParsingException("Failed to parse micro program memory: One of the given values was not found in the parsing table", e);
            }
        }

        public IDictionary<int, IMicroInstruction> ParseMicroInstructions(string microInstructions1, string microInstructions2)
        {
            using (TextReader reader1 = new StringReader(microInstructions1), reader2 = new StringReader(microInstructions2))
            {
                return ParseMicroInstructions(reader1, reader2);
            }
        }

        public IDictionary<int, IMicroInstruction> ParseMicroInstructionsFile(string filename1, string filename2)
        {
            using (TextReader reader1 = new StreamReader(filename1, encoding), reader2 = new StreamReader(filename2))
            {
                return ParseMicroInstructions(reader1, reader2);
            }
        }
    }
}
