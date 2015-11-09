using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public class Register : IRegister
    {
        public Registers Type { get; }
        public uint Value { get; }
        public Register(Registers type, uint value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
