using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public class Register : IRegister
    {
        public Registers Type { get; private set; }
        public Register(Registers type)
        {
            this.Type = type;
        }
    }
}
