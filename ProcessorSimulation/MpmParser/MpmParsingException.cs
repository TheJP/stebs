using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation.MpmParser
{
    public class MpmParsingException : Exception
    {
        public MpmParsingException() { }
        public MpmParsingException(string message) : base(message) { }
        public MpmParsingException(string message, Exception inner) : base(message, inner) { }
    }
}
