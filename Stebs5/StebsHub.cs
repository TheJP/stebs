using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using assembler.support;
using assembler;
using System.IO;
using Microsoft.Practices.Unity;

namespace Stebs5
{
    public class StebsHub : Hub
    {
        private IConstants Constants { get; }
        public StebsHub(IConstants constants)
        {
            this.Constants = constants;
        }

        public void Assemble(string source)
        {
            lock (typeof(Common))
            {
                //TODO: Improve performance
                Assembler assembler = new Assembler(string.Empty);
                using(var reader = new StreamReader(Constants.InstructionsAbsolutePath))
                {
                    var instructions = reader.ReadToEnd();
                    if (assembler.execute(source, instructions))
                    {
                        Clients.Caller.Assembled(Common.getCodeList().toString());
                    }
                    else
                    {
                        Clients.Caller.AssembleError(Common.ERROR_MESSAGE);
                    }
                }
            }
        }
    }
}
