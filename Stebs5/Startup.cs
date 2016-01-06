using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ProcessorSimulation.MpmParser;
using ProcessorDispatcher;

[assembly: OwinStartup(typeof(Stebs5.Startup))]

namespace Stebs5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Setup dependency injection
            var container = UnityConfiguration.Container;
            //Execute micro programm memory parser
            var constants = container.Resolve<IConstants>();
            container.Resolve<IMpm>().Parse(constants.InstructionsAbsolutePath, constants.Rom1AbsolutePath, constants.Rom2AbsolutePath);
            //Start dispatcher
            container.Resolve<IDispatcher>().Start();
            //Add custom hub creation
            GlobalHost.DependencyResolver.Register(typeof(StebsHub), () => container.Resolve<StebsHub>());
            app.MapSignalR();
        }
    }
}
