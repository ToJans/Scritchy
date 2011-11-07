using System;
using Scritchy.Infrastructure.Configuration;
using Scritchy.Infrastructure.Implementations;
using Scritchy.Infrastructure.Implementations.EventStorage;

namespace Scritchy.Infrastructure
{
    public class ScritchyBus : ICommandBus
    {
        public IEventStore EventStore { get; private set; }

        HandlerRegistry Registry;
        ICommandBus Bus;
        IHandlerInstanceResolver resolver;
        IEventApplier applier;
        public ScritchyBus(Func<Type,object> LoadHandler=null,IEventstoreAdapter adapter=null)
        {
            if (LoadHandler ==null)
                LoadHandler = x => Activator.CreateInstance(x);
            Registry = new ConventionBasedRegistry();
            EventStore = new Scritchy.Infrastructure.Implementations.EventStorage.EventStore(adapter: adapter);
            resolver = new HandlerInstanceResolver(EventStore,Registry,LoadHandler);
            Bus = new CommandBus(EventStore, Registry, resolver);
            applier = new EventApplier(EventStore, Registry, resolver);
        }

        public void RunCommand(object Command)
        {
            Bus.RunCommand(Command);
            applier.ApplyNewEventsToAllHandlers();
        }

        public void ApplyNewEventsToAllHandlers()
        {
            applier.ApplyNewEventsToAllHandlers();
        }
    }


}
