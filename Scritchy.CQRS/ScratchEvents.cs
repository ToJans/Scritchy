using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Scritchy.CQRS
{
    public class ScratchEvents
    {
        object state;
        List<object> PublishedEvents = new List<object>();
        public ScratchEvents(object state)
        {
            this.state = state;
        }

        public IEnumerable<object> GetPublishedEvents()
        {
            return PublishedEvents;
        }

        public static ScratchEvents operator +(ScratchEvents e, object newevent)
        {
            e.PublishedEvents.Add(newevent);
            ReflectionHelper.TryToExecuteSerializedMethodCall(e.state, newevent, "On");
            return (e);
        }

    }

}
