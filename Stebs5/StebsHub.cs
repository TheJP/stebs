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

        public void Assemble(string source)
        {
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

        public void AddFile(int parentId, string fileName)
        {
            //List<Tuple<int, int, string>>
            Clients.Caller.SetFileId(parentId + 10);
        }

        public void AddFolder(int parentId, string fileName)
        {
            Clients.Caller.SetFileId(parentId + 10);
        }

        public void DeleteFile(int parentId, int myId)
        {
            Clients.Caller.SetFileId(parentId + 10);
        }

        public void FileContent(int fileId)
        {
            Clients.Caller.FileContent("; -------------------------------------------------------------------------\n"+
                "; Conditional Jump JNZ\n"+
                "; -------------------------------------------------------------------------\n"+
                "Main:\n"+
                "Init:\n"+
                "    MOV AL,00; Imput 0\n"+
                "    MOV BL,01; Imput 1\n"+
                "    MOV CL,00; Result\n"+
                "    MOV DL,40; RAM Position\n"+
                "\n"+
                "    MOV [DL],AL\n"+
                "    INC   DL\n"+
                "    MOV [DL], BL\n"+
                "    INC DL\n"+
                "\n"+
                "Loop:      ; Perform loop\n"+
                "; SHL AL; Shift bit to the left\n" +
                "    ADD   CL,AL\n" +
                "    ADD   CL,BL\n" +
                "    MOV [DL], CL\n" +
                "\n"+
                "    MOV AL, BL\n" +
                "    MOV BL, CL\n" +
                "    MOV CL,00\n" +
                "\n"+
                "    INC DL\n" +
                "    JNO Loop; Jump if NOT Overflow\n" +
                "\n"+
                "    END\n" +
                " ; -------------------------------------------------------------------------");
        }
    }
}
