using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Practices.Unity;
using ProcessorDispatcher;
using ProcessorSimulation;
using ProcessorSimulation.Device;
using ProcessorSimulation.MpmParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stebs5
{
    /// <summary>
    /// Global configuration of all dependency injections.
    /// </summary>
    public static class UnityConfiguration
    {
        private static Lazy<UnityContainer> container = new Lazy<UnityContainer>(() =>
        {
            var container = new UnityContainer();
            container
                .RegisterInstance(container)
                //SignalR Hubs
                .RegisterType<StebsHub>()
                //Singletons
                .RegisterType<IConstants, Constants>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpmParser, MpmFileParser>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpmFileParser, MpmFileParser>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpm, Mpm>(new ContainerControlledLifetimeManager())
                .RegisterType<IProcessorSimulator, ProcessorSimulator>(new ContainerControlledLifetimeManager())
                .RegisterType<IAlu, Alu>(new ContainerControlledLifetimeManager())
                .RegisterType<IDispatcher, Dispatcher>(new ContainerControlledLifetimeManager())
                .RegisterType<IProcessorManager, ProcessorManager>(new ContainerControlledLifetimeManager())
                .RegisterType<IFileManager, FileManager>(new ContainerControlledLifetimeManager())
                .RegisterType<IPluginManager, PluginManager>(new ContainerControlledLifetimeManager())
                //Factories
                .RegisterInstance(Register.Factory)
                .RegisterInstance(DispatcherItem.Factory)
                .RegisterType<IRam, Ram>()
                .RegisterType<IProcessor, Processor>()
                .RegisterType<IDeviceManager, DeviceManager>()
                .RegisterType<IChangesCollector, ChangesCollector>();
            return container;
        });
        public static UnityContainer Container => container.Value;
    }
}