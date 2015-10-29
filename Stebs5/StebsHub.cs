using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using assembler.support;
using assembler;
using System.IO;

namespace Stebs5
{
    public class StebsHub : Hub
    {
        public void Assemble(string source)
        {
            lock (typeof(Common))
            {
                //TODO: Improve performance here:
                Assembler assembler = new Assembler("");
                var server = HttpContext.Current.Server;
                using(var reader = new StreamReader(server.MapPath("~\\bin\\Resources\\INSTRUCTION.data")))
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
