using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IProcessor
    {
        /// <summary>Event that is fired, when a register changed.</summary>
        event Action<IProcessor, IRegister> RegisterChanged;

        /// <summary>Easy way to set a register value.</summary>
        /// <param name="register">Register type</param>
        /// <param name="value">New value</param>
        void SetRegister(Registers type, byte value);
        IAlu Alu { get; }
        IRam Ram { get; }
        IDictionary<Registers, IRegister> Registers { get; }

    }
}
