using ProcessorDispatcher;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using ProcessorSimulation;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ProcessorSimulation.Device;

namespace Stebs5
{
    public class ProcessorManager : IProcessorManager
    {
        private IHubConnectionContext<dynamic> Clients { get; }
        private IDispatcher Dispatcher { get; }
        private IConstants Constants { get; }
        private readonly ConcurrentDictionary<string, IDispatcherItem> processors = new ConcurrentDictionary<string, IDispatcherItem>();

        public ProcessorManager(IDispatcher dispatcher, IConstants constants)
        {
            this.Constants = constants;
            this.Clients = GlobalHost.ConnectionManager.GetHubContext<StebsHub>().Clients;
            this.Dispatcher = dispatcher;
            this.Dispatcher.StateChanged += StateChanged;
            this.Dispatcher.FinishedStep += FinishedStep;
        }

        /// <summary>Called, when a reset or halt was processed.</summary>
        /// <param name="item">Processor, which was resetted/halted.</param>
        private void StateChanged(IDispatcherItem item, StateChange stateChange)
        {
            var group = Clients.Group(item.Guid.ToString());
            switch (stateChange)
            {
                case StateChange.SoftReset:
                    group.Reset();
                    break;
                case StateChange.Halt:
                    group.Halt();
                    break;
                case StateChange.HardReset:
                    group.HardReset();
                    break;
            }
        }

        /// <summary>Called, when a simulation step was processed.</summary>
        /// <param name="item">Processor which was simulated</param>
        /// <param name="stepSize">Simulated step size.</param>
        /// <param name="ramChanges">Changes done to the ram during the simulation step.</param>
        /// <param name="registerChanges">Changes done to the registers during the simulation step.</param>
        private void FinishedStep(IDispatcherItem item, SimulationStepSize stepSize, IReadOnlyDictionary<byte, byte> ramChanges, IReadOnlyDictionary<Registers, IRegister> registerChanges)
        {
            var guid = item.Guid.ToString();
            Clients.Group(guid).UpdateProcessor(stepSize, ramChanges, registerChanges);
        }

        public Guid CreateProcessor(string clientId)
        {
            //Remove processor, if it exists for given client id
            //This assures, that a new client does not get an existing processor
            RemoveProcessor(clientId);
            //Add new processor for given client id
            return AssureProcessorExists(clientId);
        }

        public Guid AssureProcessorExists(string clientId) =>
            //Create new processor if none exists; Use existing otherwise
            processors.AddOrUpdate(clientId, id => Dispatcher.CreateProcessor(runDelay: Constants.DefaultRunDelay), (id, processor) => processor).Guid;

        public Guid? RemoveProcessor(string clientId)
        {
            IDispatcherItem processor;
            if (processors.TryRemove(clientId, out processor))
            {
                Dispatcher.Remove(processor.Guid);
                return processor.Guid;
            }
            return null;
        }

        public void ChangeRamContent(string clientId, int[] newContent)
        {
            IDispatcherItem item;
            if(processors.TryGetValue(clientId, out item))
            {
                using (var session = item.Processor.CreateSession())
                {
                    session.RamSession.Set(newContent.Select(ram => (byte)ram).ToArray());
                }
            }
        }

        private void Update(string clientId, Func<IDispatcherItem, IDispatcherItem> update)
        {
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                Dispatcher.Update(item.Guid, update);
            }
        }

        public void Run(string clientId, SimulationStepSize stepSize) => Update(clientId, item => item.SetStepSize(stepSize).SetRunning(true));
        public void Pause(string clientId) => Update(clientId, item => item.SetRunning(false));
        public void Stop(string clientId) => Update(clientId, item => { Dispatcher.SoftReset(item.Guid); return item.SetRunning(false); });
        public void ChangeSetpSize(string clientId, SimulationStepSize stepSize) => Update(clientId, item => item.SetStepSize(stepSize));
        public void ChangeRunDelay(string clientId, TimeSpan runDelay) => Update(clientId, item => item.SetRunDelay(runDelay));

        public void Step(string clientId, SimulationStepSize stepSize)
        {
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                Dispatcher.Step(item.Guid, stepSize);
            }
        }

        private void ProcessorAction(string clientId, Action<IProcessorSession> action)
        {
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                item.Processor.Execute(action);
            }
        }

        public byte AddDevice(string clientId, IDevice device, byte? slot)
        {
            byte result = 0;
            IDispatcherItem item;
            if (processors.TryGetValue(clientId, out item))
            {
                item.Processor.Execute(session =>
                {
                    var view = new SignalrDeviceView(this, item.Guid.ToString());
                    if (!slot.HasValue) { result = session.DeviceManager.AddDevice(session.Processor, device, view); }
                    else { result = session.DeviceManager.AddDevice(session.Processor, device, view, slot.Value); }
                    view.Slot = result;
                });
            }
            return result;
        }

        public void RemoveDevice(string clientId, byte slot) =>
            ProcessorAction(clientId, session => session.DeviceManager.RemoveDevice(slot));

        public void UpdateDevice(string clientId, byte slot, IDeviceUpdate input) =>
            ProcessorAction(clientId, session => {
                var devices = session.DeviceManager.Devices;
                if (devices.ContainsKey(slot))
                {
                    devices[slot].Update(input);
                }
            });

        /// <summary>Class which is used to deliver device updates to the clien</summary>
        private class SignalrDeviceView : IDeviceView
        {
            private ProcessorManager manager;
            private string groupGuid;
            public byte Slot { get; set; }

            public SignalrDeviceView(ProcessorManager manager, string groupGuid)
            {
                this.manager = manager;
                this.groupGuid = groupGuid;
            }

            public void UpdateView(IDeviceUpdate update) => manager.Clients.Group(groupGuid).UpdateDevice(Slot, update);
        }
    }
}
