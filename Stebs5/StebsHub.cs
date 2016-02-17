using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using assembler.support;
using assembler;
using System.IO;
using Microsoft.Practices.Unity;
using ProcessorSimulation.MpmParser;
using ProcessorSimulation;
using System.Threading.Tasks;
using Stebs5.Models;
using Stebs5Model;
using ProcessorSimulation.Device;

namespace Stebs5
{
    /// <summary>
    /// Cnentral communication interface between client and server.
    /// </summary>
    [Authorize]
    public class StebsHub : Hub
    {
        private IConstants Constants { get; }
        private IMpm Mpm { get; }
        private IProcessorManager Manager { get; }
        private IFileManager FileManager { get; }
        private IPluginManager PluginManager { get; }

        public StebsHub(IConstants constants, IMpm mpm, IProcessorManager manager, IFileManager fileManager, IPluginManager pluginManager)
        {
            this.Constants = constants;
            this.Mpm = mpm;
            this.Manager = manager;
            this.FileManager = fileManager;
            this.PluginManager = pluginManager;
        }

        private void RemoveProcessor()
        {
            var guid = Manager.RemoveProcessor(Context.ConnectionId);
            if (guid != null) { Groups.Remove(Context.ConnectionId, guid.Value.ToString()); }
        }

        private void CreateProcessor()
        {
            var guid = Manager.CreateProcessor(Context.ConnectionId).ToString();
            Groups.Add(Context.ConnectionId, guid);
        }

        private void AssureProcessorExists()
        {
            var guid = Manager.AssureProcessorExists(Context.ConnectionId).ToString();
            Groups.Add(Context.ConnectionId, guid);
        }

        public override Task OnConnected()
        {
            CreateProcessor();
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            AssureProcessorExists();
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            RemoveProcessor();
            return base.OnDisconnected(stopCalled);
        }

        public InitialiseViewModel Initialise() => new InitialiseViewModel(
            instructions: Mpm.Instructions,
            registers: RegistersExtensions.GetValues().Select(type => type.ToString()),
            deviceTypes: PluginManager.DevicePlugins.Values
                .ToDictionary(device => device.PluginId, device => new DeviceViewModel(device.Name, device.PluginId)),
            processorId: Manager.GetProcessorId(Context.ConnectionId)?.ToString()
        );

        /// <summary>
        /// Assemble the given source from the client.
        /// The assembly file does not have to be saved: That's why the source is submitted as string,
        /// so an unfinished program can be assembled and tested before saving it.
        /// </summary>
        /// <param name="source">Source code in assembly.</param>
        public void Assemble(string source)
        {
            //The assembling is globaly locked, because of the way the assembler is implemented
            lock (typeof(Common))
            {
                Assembler assembler = new Assembler(string.Empty);
                if (assembler.execute(source, Mpm.RawInstructions))
                {
                    Manager.Stop(Context.ConnectionId);
                    Clients.Caller.Assembled(Common.getCodeList().toString(), Common.getRam(), assembler.getCodeToLineArr());
                    Manager.ChangeRamContent(Context.ConnectionId, Common.getRam());
                }
                else
                {
                    Clients.Caller.AssembleError(Common.ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// Executes given delegate only, if a valid step size is passed.
        /// </summary>
        /// <param name="stepSize">Step size to check and pass.</param>
        /// <param name="action">Action which is called with valid step size.</param>
        private void DoWithCheckedStepSize(SimulationStepSize stepSize, Action<SimulationStepSize> action)
        {
            if (Enum.IsDefined(typeof(SimulationStepSize), stepSize)) { action(stepSize); }
        }

        public void Run(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.Run(Context.ConnectionId, s));
        public void ChangeStepSize(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.ChangeSetpSize(Context.ConnectionId, s));
        public void Pause() => Manager.Pause(Context.ConnectionId);
        public void Stop() => Manager.Stop(Context.ConnectionId);
        public void Reset() => Manager.Reset(Context.ConnectionId);
        public void Step(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.Step(Context.ConnectionId, s));
        /// <summary>Sets the run delay in milliseconds. The absolute minimum is defined</summary>
        /// <param name="delay"></param>
        public void ChangeRunDelay(uint delay)
        {
            var value = TimeSpan.FromMilliseconds(delay);
            value = value < Constants.MinimalRunDelay ? Constants.MinimalRunDelay : value;
            Manager.ChangeRunDelay(Context.ConnectionId, value);
        }

        /// <summary>
        /// Add a node to the users filesystem.
        /// </summary>
        /// <param name="parentId">Parent Id</param>
        /// <param name="fileName">name of the file to create</param>
        /// <param name="isFolder">true if node is a folder</param>
        /// <returns>The actualized filesystem will be returned</returns>
        public FileSystemViewModel AddNode(long parentId, string fileName, bool isFolder) => FileManager.AddNode(Context.User, parentId, fileName, isFolder);

        /// <summary>
        /// Change a node (folder/File) name by id.
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="newNodeName">the new name</param>
        /// <param name="isFolder">true if node is a folder</param>
        /// <returns>The actualized filesystem will be returned</returns>
        public FileSystemViewModel ChangeNodeName(long nodeId, string newNodeName, bool isFolder) => FileManager.ChangeNodeName(Context.User, nodeId, newNodeName);

        /// <summary>
        /// Delete a node (file/folder) by id.
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="isFolder">true if node is a folder</param>
        /// <returns>The actualized filesystem will be returned</returns>
        public FileSystemViewModel DeleteNode(long nodeId, bool isFolder) => FileManager.DeleteNode(Context.User, nodeId);

        /// <summary>
        /// Load the filesystme from the db.
        /// </summary>
        /// <returns>filesystem of this user</returns>
        public FileSystemViewModel GetFileSystem() => FileManager.GetFileSystem(Context.User);

        /// <summary>
        /// Load a fileContent (node) form the DB
        /// </summary>
        /// <param name="fileId">the id of the node</param>
        /// <returns>string containing the fileContent</returns>
        public string GetFileContent(long fileId) => FileManager.GetFileContent(Context.User, fileId);

        /// <summary>
        /// Save the fileContent to the DB
        /// </summary>
        /// <param name="fileId">node ID</param>
        /// <param name="fileContent">the new Content of this file</param>
        public void SaveFileContent(long fileId, string fileContent) => FileManager.SaveFileContent(Context.User, fileId, fileContent);

        /// <summary>Adds a device to the processor of the calling client.</summary>
        /// <param name="deviceId"></param>
        /// <param name="slot">If the slot is 0, a free slot number is chosen.</param>
        /// <returns>Slot number, at which the device was placed.</returns>
        public AddedDeviceViewModel AddDevice(string deviceId, byte? slot)
        {
            if (PluginManager.DevicePlugins.ContainsKey(deviceId))
            {
                var plugin = PluginManager.DevicePlugins[deviceId];
                var device = plugin.CreateDevice();
                var givenSlot = Manager.AddDevice(Context.ConnectionId, device, slot);
                return new AddedDeviceViewModel(givenSlot, plugin.DeviceTemplate(givenSlot));
            }
            return new AddedDeviceViewModel(false);
        }

        /// <summary>Removes the device at the given slot from the processor of the calling client.</summary>
        /// <param name="slot"></param>
        public void RemoveDevice(byte slot) => Manager.RemoveDevice(Context.ConnectionId, slot);

        /// <summary>
        /// Update the device with new information from the client.
        /// This can e.g. be ui interactions with the device.
        /// </summary>
        /// <param name="slot">Device slot in the processor.</param>
        /// <param name="input">Update information.</param>
        public void UpdateDevice(byte slot, string input) => Manager.UpdateDevice(Context.ConnectionId, slot, input);
    }
}
