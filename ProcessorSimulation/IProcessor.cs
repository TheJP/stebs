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

        IAlu Alu { get; }
        IRam Ram { get; }
        IDictionary<Registers, IRegister> Registers { get; }

        /// <summary>Create session, with which the processor state can be modified.</summary>
        /// <returns>Session instance</returns>
        /// <remarks>This method can block, because only one session should exist and it should be used by one thread only.</remarks>
        IProcessorSession createSession();
    }
}
