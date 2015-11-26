using ProcessorDispatcher;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace Stebs5
{
    public class ProcessorManager : IProcessorManager
    {
        private IDispatcher Dispatcher { get; }
        private ConcurrentDictionary<string, IDispatcherItem> processors = new ConcurrentDictionary<string, IDispatcherItem>();

        public ProcessorManager(IDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public void CreateProcessor(string clientId)
        {
            //Remove processor, if it exists for given client id
            //This assures, that a new client does not get an existing processor
            RemoveProcessor(clientId);
            //Add new processor for given client id
            AssureProcessorExists(clientId);
        }

        public void AssureProcessorExists(string clientId) =>
            //Create new processor if none exists; Use existing otherwise
            processors.AddOrUpdate(clientId, id => Dispatcher.CreateProcessor(), (id, processor) => processor);

        public void RemoveProcessor(string clientId)
        {
            IDispatcherItem processor;
            if (processors.TryRemove(clientId, out processor))
            {
                Dispatcher.Remove(processor.Guid);
            }
        }

        public void ChangeRamContent(string clientId, int[] newContent)
        {
            IDispatcherItem item;
            if(processors.TryGetValue(clientId, out item))
            {
                using (var session = item.Processor.CreateSession())
                {
                    session.RamSession.Set(newContent.Select(i => (byte)i).ToArray());
                }
            }
        }
    }
}
