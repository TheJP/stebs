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

        public override bool Equals(object obj)
        {
            if(obj == null) { return false; }
            if(ReferenceEquals(this, obj)) { return true; }
            if(GetType() != obj.GetType()) { return false; }
            Register register = (Register) obj;
            return Type == register.Type && Value == register.Value;
        }

        public override int GetHashCode()
        {
            return ((int) Type) << 8 ^ (int) Value;
        }
    }
}
