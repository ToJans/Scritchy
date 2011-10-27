using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.CQRS.Infrastructure
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

        public void ApplyEventsToInstance(object instance, IEnumerable<object> events)
        {
            var instancetype = instance.GetType();
            foreach (var evt in events)
            {
                this.handlerregistry[instancetype, evt.GetType()](instance, evt);
            }
        }

        public void ApplyNewEventsToAllHandlers()
        {
            foreach (var e in eventstore.GetNewEventsSincePreviousRead())
            {
                foreach(var key in handlerregistry.RegisteredHandlers.Where(x=>x.MessageType == e.GetType()))
                {
                    if(typeof(AR).IsAssignableFrom(key.InstanceType))
                        continue;
                    object handler=resolver.ResolveHandlerFromType(key.InstanceType);
                    handlerregistry[key](handler, e);
                }
            }
        }
    }
}
