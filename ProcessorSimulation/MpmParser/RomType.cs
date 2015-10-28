using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    public class RomType
    {
        public static RomType Rom1 { get; } = new RomType(microcode =>
        {
            //var parsed = int.Parse(microcode, NumberStyles.HexNumber);
            var address = (microcode >> 20) & 0xFFF;
            var nextAddress = (NextAddress)((microcode >> 18) & 0x3);
            var clearInterrupt = ((microcode >> 17) & 0x1) == 1;
            var enableValue = ((microcode >> 16) & 0x1) == 1;
            var value = (microcode >> 4) & 0xFFF;
            var affected = ((microcode >> 3) & 1) == 1;
            var criterion = (JumpCriterion)((microcode >> 0) & 7);
            return null;
        });
        public static RomType Rom2 { get; } = new RomType(microcode =>
        {
            return null;
        });

        public Func<int, IMicroInstruction> Converter { get; }

        private RomType(Func<int, IMicroInstruction> converter)
        {
            this.Converter = converter;
        }
    }
}
