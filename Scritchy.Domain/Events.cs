using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Scritchy.Domain
{
    public class Events
    {
        object state;
        List<object> PublishedEvents = new List<object>();
        Action<object> TryApplyEvent;
        public Events(object state, Action<object> TryApplyEvent)        
        {
            this.state = state;
            this.TryApplyEvent = TryApplyEvent;
        }

        public IEnumerable<object> GetPublishedEvents()
        {
            return PublishedEvents;
        }

        public static Events operator +(Events e, object newevent)
        {
            e.PublishedEvents.Add(newevent);
            e.TryApplyEvent(newevent);
            return (e);
        }

    }

}
