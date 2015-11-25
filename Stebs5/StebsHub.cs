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

namespace Stebs5
{
    public class StebsHub : Hub
    {
        private IConstants Constants { get; }
        private IMpm Mpm { get; }
        public StebsHub(IConstants constants, IMpm mpm)
        {
            this.Constants = constants;
            this.Mpm = mpm;
        }

        public void Assemble(string source)
        {
            lock (typeof(Common))
            {
                Assembler assembler = new Assembler(string.Empty);
                if (assembler.execute(source, Mpm.RawInstructions))
                {
                    Clients.Caller.Assembled(Common.getCodeList().toString(),
                        Common.getRam(), assembler.getCodeToLineArr());
                }
                else
                {
                    Clients.Caller.AssembleError(Common.ERROR_MESSAGE);
                }
            }
        }

        public void GetInstructions()
        {
            Clients.Caller.Instructions(Mpm.Instructions);
        }

        public void AddFile(int parentId, string fileName)
        {
            //List<Tuple<int, int, string>> 
            Clients.Caller.SetFileId(parentId + 10);
        }
        public void AddFolder(int parentId, string fileName)
        {
            Clients.Caller.SetFileId(parentId += 10);
        }

        public void LoadRegisters()
        {
            Clients.Caller.SetAvailableRegisters(((Registers[])Enum.GetValues(typeof(Registers)))
                .Select(type => type.ToString()));
        }
    }
}
