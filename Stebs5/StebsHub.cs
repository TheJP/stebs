using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Stebs5
{
    public class StebsHub : Hub
    {
        public void Hello(string name)
        {
            Clients.All.hello($"Hello {name}");
        }
    }
}
