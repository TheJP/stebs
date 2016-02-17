using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ProcessorSimulation.MpmParser;
using ProcessorDispatcher;
using Stebs5Model;
using System.Data.Entity;
using Stebs5Model.Migrations;

[assembly: OwinStartup(typeof(Stebs5.Startup))]
namespace Stebs5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Initialize / Update database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StebsDbContext, Configuration>());
            ConfigureAuth(app);
            //Setup dependency injection
            var container = UnityConfiguration.Container;
            var constants = container.Resolve<IConstants>();
            //Load all device plugins
            LoadPluginAssemblies(constants);
            AddAllDevicePlugins(container.Resolve<IPluginManager>());
            //Execute micro programm memory parser
            container.Resolve<IMpm>().Parse(constants.InstructionsAbsolutePath, constants.Rom1AbsolutePath, constants.Rom2AbsolutePath);
            //Start dispatcher
            container.Resolve<IDispatcher>().Start();
            //Add custom hub creation
            GlobalHost.DependencyResolver.Register(typeof(StebsHub), () => container.Resolve<StebsHub>());
            app.MapSignalR();
        }
    }
}
