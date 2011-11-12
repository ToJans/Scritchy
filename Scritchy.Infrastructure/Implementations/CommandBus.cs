using System.Linq;
using System.Reflection;
using Scritchy.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{

    public class CommandBus : ICommandBus
    {
        public IEventStore eventstore;
        public HandlerRegistry handlerregistry = new HandlerRegistry();
        public IHandlerInstanceResolver resolver;

        static readonly Helpers.Synchronizer<string> ArLocker = new Helpers.Synchronizer<string>();

        public CommandBus(IEventStore eventstore, HandlerRegistry handlerregistry, IHandlerInstanceResolver resolver)
        {
            this.eventstore = eventstore;
            this.handlerregistry = handlerregistry;
            this.resolver = resolver;
        }

        public void RunCommand(object Command)
        {
            var keys = handlerregistry.RegisteredHandlers.Where(x => x.MessageType == Command.GetType());
            if (!keys.Any() )
                throw new InvalidOperationException("No handler found for the commands of type " + Command.GetType().Name);
            foreach (var key in keys)
            {
                var id = Command.GetType().GetProperty(key.InstanceType.Name + "Id").GetValue(Command, null) as string;
                Guid index = Guid.NewGuid();
                AR ar = null;
                try
                {
                    var handler = handlerregistry[key];
                    using (var mylock = ArLocker.Lock(id))
                    {
                        lock (mylock)
                        {
                            ar = resolver.LoadARSnapshot(key.InstanceType, id);
                            handler(ar, Command);
                        }
                    }
                }
                catch (TargetInvocationException ex)
                {
                    throw new FailedCommandException(ex.InnerException, Command);
                }
                if (!eventstore.SaveEvents(ar.Changes.GetPublishedEvents()))
                    throw new SaveEventsException { Events = ar.Changes.GetPublishedEvents() };
            }
        }

    }
}
