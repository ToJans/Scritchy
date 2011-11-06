using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Scritchy.Infrastructure.Exceptions;

namespace Scritchy.Infrastructure.Implementations
{

    public class CommandBus:ICommandBus
    {
        public IEventStore eventstore;
        public HandlerRegistry handlerregistry = new HandlerRegistry();
        public IHandlerInstanceResolver resolver;

        public CommandBus(IEventStore eventstore, HandlerRegistry handlerregistry, IHandlerInstanceResolver resolver)
        {
            this.eventstore = eventstore;
            this.handlerregistry = handlerregistry;
            this.resolver = resolver;
        }

        public void RunCommand(object Command)
        {
            var key = handlerregistry.RegisteredHandlers.Where(x => x.MessageType == Command.GetType()).First();
            var id = Command.GetType().GetProperty(key.InstanceType.Name + "Id").GetValue(Command, null) as string;
            var ar = resolver.LoadARSnapshot(key.InstanceType, id);

            var handler = handlerregistry[key];
            try
            {
                handler(ar, Command);
            }
            catch (TargetInvocationException ex)
            {
                throw new FailedCommandException(ex.InnerException,Command);
            }
            if (!eventstore.SaveEvents(ar.Changes.GetPublishedEvents()))
                throw new SaveEventsException {Events = ar.Changes.GetPublishedEvents()};
        }

    }
}
