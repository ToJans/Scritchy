using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Scritchy.CQRS.Infrastructure;

namespace Scritchy.CQRS
{
    public class Events
    {
        object state;
        List<object> PublishedEvents = new List<object>();
        HandlerRegistry handlerregistry;
        public Events(object state,HandlerRegistry handlerregistry)
        {
            this.state = state;
            this.handlerregistry = handlerregistry;
        }

        public IEnumerable<object> GetPublishedEvents()
        {
            return PublishedEvents;
        }

        public static Events operator +(Events e, object newevent)
        {
            e.PublishedEvents.Add(newevent);
            if (e.handlerregistry.ContainsHandler(e.state.GetType(), newevent.GetType()))
                e.handlerregistry[e.state.GetType(), newevent.GetType()].Invoke(e.state, newevent);
            return (e);
        }

    }

}
