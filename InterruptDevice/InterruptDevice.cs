using Newtonsoft.Json;
using ProcessorSimulation.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterruptDevice
{
    public class InterruptDevice : DefaultDevice
    {
        private int interruptInterval = 10;
        private bool active = false;

        public override void Detached()
        {
            active = false;
        }

        public override void Update(string input)
        {
            switch (input)
            {
                case "InterruptOnce":
                    Interrupt();
                    break;
                case "ActivateInterrupts":
                    active = true;
                    break;
                case "DisableInterrupts":
                    active = false;
                    break;
                default:
                    var command = JsonConvert.DeserializeObject<ChangeIntervalCommand>(input);
                    interruptInterval = command.NewInterval;
                    break;
            }
        }
    }
}
