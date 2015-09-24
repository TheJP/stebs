using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public interface IProcessor
    {
        event Action<IProcessor, IRegister> RegisterChanged;
    }
}
