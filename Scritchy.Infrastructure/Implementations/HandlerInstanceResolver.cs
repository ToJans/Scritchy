using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{
    public class HandlerInstanceResolver : IHandlerInstanceResolver
    {
        IEventStore eventsource;
        HandlerRegistry handlerregistry;
        Func<Type, object> LoadHandler;

        public HandlerInstanceResolver(IEventStore eventsource,HandlerRegistry handlerregistry,Func<Type,object> LoadHandler)
        {
            this.eventsource = eventsource;
            this.handlerregistry = handlerregistry;
            this.LoadHandler = LoadHandler;
        }

        public void ApplyEventsToInstance(object instance)
        {
            var instancetype = instance.GetType();
            foreach (var evt in eventsource.GetNewEvents(instance))
            {
                this.handlerregistry[instancetype, evt.GetType()](instance, evt);
            }
        }


        public AR LoadARSnapshot(Type t, string Id)
        {
            var ar = Activator.CreateInstance(t) as AR;
            ar.Id = Id;
            ar.TryApplyEvent = x => {
                if (handlerregistry.ContainsHandler(t, x.GetType()))
                    handlerregistry[t, x.GetType()](ar, x);
            };
            ApplyEventsToInstance(ar);
            return ar;
        }

        public object ResolveHandlerFromType(Type t)
        {
            return LoadHandler(t);
        }
    }
}
