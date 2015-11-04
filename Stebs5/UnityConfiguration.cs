using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Practices.Unity;
using ProcessorSimulation;
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
                .RegisterType<StebsHub>()
                .RegisterInstance<IProcessorFactory>(new ProcessorFactory())
                .RegisterInstance<IConstants>(new Constants());
            return container;
        });
        public static UnityContainer Container { get; } = container.Value;
    }
}