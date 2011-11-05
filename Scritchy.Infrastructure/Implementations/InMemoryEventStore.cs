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


        public bool SaveEvents(IEnumerable<object> events)
        {
            PublishedEvents.AddRange(events);
            return true;
        }

        Dictionary<object,int> ContextPointers = new Dictionary<object,int>();

         object GlobalContext = new object();

        public IEnumerable<object> GetNewEvents(object Instance=null,object EnumeratorContext=null)
        {
            EnumeratorContext = EnumeratorContext ?? Instance ?? GlobalContext;
            if (!ContextPointers.ContainsKey(EnumeratorContext))
            {
                ContextPointers.Add(EnumeratorContext, 0);
            }
            while (ContextPointers[EnumeratorContext] < PublishedEvents.Count)
            {
                object res = PublishedEvents[ContextPointers[EnumeratorContext]++];
                if (Instance == null)
                {
                    yield return res;
                } else if (handlerregistry.ContainsHandler(Instance.GetType(), res.GetType()))
                {
                    var ar = Instance as AR;
                    if (ar == null)
                        yield return res;
                    else
                    {
                        var msgid = res.GetType().GetProperty(Instance.GetType().Name + "Id").GetValue(res,null) as string;
                        if (ar.Id == msgid)
                            yield return res;
                    }
                }
            }
        }
    }
}
