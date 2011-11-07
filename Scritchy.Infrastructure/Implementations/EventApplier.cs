using System.Linq;
using System.Reflection;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{
    public class EventApplier : IEventApplier
    {
        IEventStore eventstore;
        HandlerRegistry handlerregistry;
        IHandlerInstanceResolver resolver;

        public EventApplier(IEventStore eventstore, HandlerRegistry handlerregistry, IHandlerInstanceResolver resolver)
        {
            this.eventstore = eventstore;
            this.handlerregistry = handlerregistry;
            this.resolver = resolver;
        }

        public void ApplyEventsToInstance(object instance)
        {
            var instancetype = instance.GetType();
            var enumeratorcontext = instance is AR ? instance : instancetype;
            foreach (var evt in eventstore.GetNewEvents(instance,enumeratorcontext))
            {
                if (!this.handlerregistry.ContainsHandler(instancetype,evt.GetType()))
                    continue;
                var handler = this.handlerregistry[instancetype, evt.GetType()];
                try
                {
                    handler(instance, evt);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
        }

        public void ApplyNewEventsToAllHandlers()
        {
            foreach(var handlertype in handlerregistry.RegisteredHandlers
                .Where(x=>!typeof(AR).IsAssignableFrom(x.InstanceType))
                .Select(x=>x.InstanceType).Distinct())
            {
                object handler=resolver.ResolveHandlerFromType(handlertype);
                ApplyEventsToInstance(handler);
            }
        }
    }
}
