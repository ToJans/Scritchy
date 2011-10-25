using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Scritchy.CQRS
{
    public class Events
    {
        object state;
        List<object> PublishedEvents = new List<object>();
        public Events(object state)
        {
            this.state = state;
        }

        public IEnumerable<object> GetPublishedEvents()
        {
            return PublishedEvents;
        }

        public static Events operator +(Events e, object newevent)
        {
            e.PublishedEvents.Add(newevent);
            ReflectionHelper.TryToExecuteSerializedMethodCall(e.state, newevent, "On");
            return (e);
        }

    }

}
