using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorSimulation
{
    public class Ram : IRam
    {
        private volatile ImmutableDictionary<byte, byte> data;

        public Ram()
        {
            var dataBuilder = ImmutableDictionary.CreateBuilder<byte, byte>();
            for(byte i = 0; i <= 0xFF; ++i)
            {
                dataBuilder.Add(i, 0);
            }
            data = dataBuilder.ToImmutable();
        }
    }
}
