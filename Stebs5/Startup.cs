using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ProcessorSimulation.MpmParser;

[assembly: OwinStartup(typeof(Stebs5.Startup))]

namespace Stebs5
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfiguration.Container;
            var constants = container.Resolve<IConstants>();
            container.Resolve<IMpm>().Parse(constants.InstructionsAbsolutePath, constants.Rom1AbsolutePath, constants.Rom2AbsolutePath);
            GlobalHost.DependencyResolver.Register(typeof(StebsHub), () => container.Resolve<StebsHub>());
            app.MapSignalR();
        }
    }
}
