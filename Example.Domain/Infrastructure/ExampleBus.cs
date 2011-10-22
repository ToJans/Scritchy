using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Implementation;
using Example.Domain.Implementation.Commands;
using Example.Domain.Implementation.Events;
using Example.Domain.Implementation.Readmodel;
using Scritchy.CQRS;

namespace Example.Domain.Infrastructure
{
    public class ExampleBus : ScratchBus
    {
        Func<Type, object> LoadHandler;

        public ExampleBus(Func<Type,object> LoadHandler)
        {
            this.LoadHandler = LoadHandler;

            var cmdTypes = typeof(AddItems).AllPublicTypesInNameSpace();
            var ARTypes = typeof(StockItem).AllPublicTypesInNameSpace();
            var EventTypes = typeof(ItemsAdded).AllPublicTypesInNameSpace();
            var readmodelhandlers = typeof(StockDictionaryHandler).AllPublicTypesInNameSpace().Where(x=>x.Name.EndsWith("Handler"));
            RegisterCommandHandlers(ARTypes, cmdTypes);
            RegisterEventHandlers(ARTypes, EventTypes);
            RegisterEventHandlers(readmodelhandlers,EventTypes);
        }

        public List<object> PublishedEvents = new List<object>();

        protected override object LoadInstanceFromTypeWithId(System.Type type, string id)
        {
            var AR = Activator.CreateInstance(type) as ScratchAR;
            AR.Id = id;
            var filter = base.EventFilterForARInstance(type, id);
            ApplyEventsToInstance(AR, PublishedEvents.Where(x => filter(x)));
            return AR;
        }

        protected override void SaveEvents(IEnumerable<object> events)
        {
            PublishedEvents.AddRange(events);
        }

        protected override object ResolveHandlerFromType(Type t)
        {
            return LoadHandler(t);
        }
    }
}
