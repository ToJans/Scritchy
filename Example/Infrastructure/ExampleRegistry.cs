using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain;
using Example.Domain.Commands;
using Example.Domain.Events;
using Example.Domain.Readmodel;
using Scritchy.CQRS.Infrastructure;
using Scritchy.CQRS;

namespace Example.Infrastructure
{
    public class ExampleRegistry : HandlerRegistry
    {
        public ExampleRegistry()
        {
            var ARTypes = typeof(StockItem).AllPublicTypesInNameSpace().Where(x=>typeof(AR).IsAssignableFrom(x));
            var cmdTypes = typeof(AddItems).AllPublicTypesInNameSpace();
            var EventTypes = typeof(ItemsAdded).AllPublicTypesInNameSpace();
            var readmodelhandlers = typeof(StockDictionaryHandler).AllPublicTypesInNameSpace().Where(x=>x.Name.EndsWith("Handler"));
            RegisterHandlers(ARTypes, cmdTypes);
            RegisterHandlers(ARTypes, EventTypes,"On");
            RegisterHandlers(readmodelhandlers,EventTypes,"On");
        }
    }
}
