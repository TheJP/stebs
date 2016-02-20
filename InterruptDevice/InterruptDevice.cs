using Newtonsoft.Json;
using ProcessorSimulation.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InterruptDevice
{
    /// <summary>Device, which can send single interrupts or regular timed interrupts.</summary>
    public class InterruptDevice : DefaultDevice
    {
        private const int MinimalInterval = 20;
        private Timer timer = new Timer();

        public override void Attached()
        {
            timer.Elapsed += (sender, e) => Interrupt();
        }

        public override void Detached() => timer.Stop();

        public override void Update(string input)
        {
            switch (input)
            {
                case "InterruptOnce":
                    Interrupt();
                    break;
                case "ActivateInterrupts":
                    timer.Start();
                    break;
                case "DisableInterrupts":
                    timer.Stop();
                    break;
                default:
                    try
                    {
                        var command = JsonConvert.DeserializeObject<ChangeIntervalCommand>(input);
                        timer.Interval = Math.Max(command.NewInterval, MinimalInterval);
                    }
                    catch (Exception) { }
                    break;
            }
        }
    }
}
