using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;

[assembly: OwinStartup(typeof(Stebs5.Startup))]

namespace Stebs5
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfiguration.Container;
            GlobalHost.DependencyResolver.Register(typeof(StebsHub), () => container.Resolve<StebsHub>());
            app.MapSignalR();
        }
    }
}
