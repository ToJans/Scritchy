using System.Linq;
using System.Reflection;
using Scritchy.Infrastructure.Exceptions;
using System;

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
            var key = handlerregistry.RegisteredHandlers.Where(x => x.MessageType == Command.GetType()).FirstOrDefault();
            if (key == null)
                throw new InvalidOperationException("No handler found for the commands of type " + Command.GetType().Name);
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
