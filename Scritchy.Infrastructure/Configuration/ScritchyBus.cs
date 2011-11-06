using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scritchy.Infrastructure.Implementations;

namespace Scritchy.Infrastructure.Configuration
{
    public class ScritchyBus : ICommandBus
    {
        HandlerRegistry Registry;
        ICommandBus Bus;
        IEventStore eventstore;
        IHandlerInstanceResolver resolver;
        IEventApplier applier;
        public ScritchyBus(Func<Type,object> LoadHandler=null)
        {
            if (LoadHandler ==null)
                LoadHandler = x => Activator.CreateInstance(x);
            Registry = new ConventionBasedRegistry();
            eventstore = new InMemoryEventStore(Registry);
            resolver = new HandlerInstanceResolver(eventstore,Registry,LoadHandler);
            Bus = new CommandBus(eventstore, Registry, resolver);
            applier = new EventApplier(eventstore, Registry, resolver);
        }

        public void RunCommand(object Command)
        {
            Bus.RunCommand(Command);
            applier.ApplyNewEventsToAllHandlers();
        }
    }


}
