using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{
    public class InMemoryEventStore : IEventStore
    {
        HandlerRegistry handlerregistry;

        public InMemoryEventStore(HandlerRegistry handlerregistry)
        {
            this.handlerregistry = handlerregistry;
        }

        List<object> PublishedEvents = new List<object>();

        public IEnumerable<object> EventsForInstance(object Instance)
        {
            var types = handlerregistry.RegisteredHandlers.Where(x => x.InstanceType == Instance.GetType()).Select(x => x.MessageType).ToList();
            Predicate<object> eventallowed = obj => true;
            var ar = Instance as AR;
            if (ar != null)
            {
                eventallowed = obj => obj.GetType().GetProperty(ar.GetType().Name + "Id").GetValue(obj,null) as string == ar.Id;
            }
            return from x in PublishedEvents
                   where types.Contains(x.GetType()) && eventallowed(x)
                   select x;
        }

        public bool SaveEvents(IEnumerable<object> events)
        {
            PublishedEvents.AddRange(events);
            return true;
        }

        int pointer = 0;

        public IEnumerable<object> GetNewEventsSincePreviousRead()
        {
            while (pointer < PublishedEvents.Count)
            {
                yield return PublishedEvents[pointer++];
            }
        }
    }
}
