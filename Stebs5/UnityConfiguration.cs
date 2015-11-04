using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Practices.Unity;
using ProcessorSimulation;
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
                //SignalR Hubs
                .RegisterType<StebsHub>()
                //Singletons
                .RegisterType<IProcessorFactory, ProcessorFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<IConstants, Constants>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpmParser, MpmFileParser>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpmFileParser, MpmFileParser>(new ContainerControlledLifetimeManager())
                .RegisterType<IMpm, Mpm>(new ContainerControlledLifetimeManager())
                .RegisterType<IProcessorSimulator, ProcessorSimulator>(new ContainerControlledLifetimeManager());
            return container;
        });
        public static UnityContainer Container { get; } = container.Value;
    }
}