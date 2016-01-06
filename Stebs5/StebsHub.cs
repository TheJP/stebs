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

namespace Stebs5
{
    /// <summary>
    /// Cnentral communication interface between client and server.
    /// </summary>
    public class StebsHub : Hub
    {
        private IConstants Constants { get; }
        private IMpm Mpm { get; }
        private IProcessorManager Manager { get; }

        public StebsHub(IConstants constants, IMpm mpm, IProcessorManager manager)
        {
            this.Constants = constants;
            this.Mpm = mpm;
            this.Manager = manager;
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
            if(Enum.IsDefined(typeof(SimulationStepSize), stepSize)) { action(stepSize); }
        }

        public void Run(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.Run(Context.ConnectionId, s));
        public void ChangeStepSize(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.ChangeSetpSize(Context.ConnectionId, s));
        public void Pause() => Manager.Pause(Context.ConnectionId);
        public void Stop() => Manager.Stop(Context.ConnectionId);
        public void Step(SimulationStepSize stepSize) => DoWithCheckedStepSize(stepSize, s => Manager.Step(Context.ConnectionId, s));
        /// <summary>Sets the run delay in milliseconds. The absolute minimum is defined</summary>
        /// <param name="delay"></param>
        public void ChangeRunDelay(uint delay)
        {
            var value = TimeSpan.FromMilliseconds(delay);
            value = value < Constants.MinimalRunDelay ? Constants.MinimalRunDelay : value;
            Manager.ChangeRunDelay(Context.ConnectionId, value);
        }
        public void GetInstructions() => Clients.Caller.Instructions(Mpm.Instructions);
        public void GetRegisters() => Clients.Caller.Registers(RegistersExtensions.GetValues().Select(type => type.ToString()));

        public void AddNode(int parentId, string fileName, bool isFolder)
        {
            //List<Tuple<int, int, string>>
            //.done() used to reload data
        }

        public void ChangeNodeName(int nodeId, string newNodeName, bool isFolder)
        {
            //.done() used to reload data
        }

        public void DeleteNode(int nodeId, bool isFolder)
        {
            //.done() used to reload data
        }

        public void GetFileSystem()
        {
            //return Filesystem
        }

        public string GetFileContent(int fileId)
        {
            return (@"; -------------------------------------------------------------------------
; Fibonacci
; -------------------------------------------------------------------------
Main:
Init:
    MOV AL, 00 ; Initial value fib(0)
    MOV BL, 01 ; Initial value fib(1)
    MOV CL, 00 ; Result
    MOV DL, 40 ; RAM Position

    MOV [DL], AL
    INC DL
    MOV [DL], BL

Loop:

    MOV CL, 00

    ; Fibonacci step: CL = AL + BL => fib(n) = fib(n - 2) + fib(n - 1)
    ADD CL, AL
    ADD CL, BL
    INC DL
    MOV [DL], CL

    ; Prepare AL and BL for the next step
    MOV AL, BL
    MOV BL, CL

    JMP Loop  ; Next loop iteration

    END
; -------------------------------------------------------------------------");
        }
    }
}
