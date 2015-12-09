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
        /// <summary>Event, that is fired, when the halt instruction was simulated.</summary>
        event Action<IProcessor> Halted;

        IAlu Alu { get; }
        IReadOnlyRam Ram { get; }
        IDictionary<Registers, IRegister> Registers { get; }
        /// <summary>Returns the initial value of the stack pointer.</summary>
        uint InitialStackPointer { get; }

        /// <summary>Create session, with which the processor state can be modified.</summary>
        /// <returns>Session instance</returns>
        /// <remarks>This method can block, because only one session should exist and it should be used by one thread only.</remarks>
        IProcessorSession CreateSession();

        /// <summary>Notifies, that the simulaton is stopped by a halt instruction.</summary>
        void NotifyHalt();
    }
}
