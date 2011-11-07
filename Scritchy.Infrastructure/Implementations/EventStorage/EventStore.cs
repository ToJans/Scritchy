using System;
using System.Collections.Generic;
using System.Linq;
using Scritchy.Infrastructure.Implementations.EventStorage.Adapters;
using Scritchy.Infrastructure.Implementations.EventStorage.Models;

namespace Scritchy.Infrastructure.Implementations.EventStorage
{
    public interface IEventstoreAdapter
    {
        bool SaveEvent(EventBlob blob,IEnumerable<EventHeader> Headers);
        IEnumerable<EventBlob> FindAll(long fromid=0);
        IEnumerable<EventBlob> FindAllWithHeader(string name,string value,long fromid=0);
    }

    public interface ISerializer
    {
        void Serialize(object instance, ref EventBlob blob);
        object Deserialize(EventBlob blob);
    }

    public class EventStore: Scritchy.Infrastructure.IEventStore 
    {
        IEventstoreAdapter adapter;
        ISerializer serializer;

        public EventStore(IEventstoreAdapter adapter=null,ISerializer serializer=null)
        {
            if (adapter == null)
            {
                if (DBEventstoreAdapter.CanCallParameterLessConstructor())
                    adapter = new DBEventstoreAdapter();
                else
                    adapter = new InMemoryEventstoreAdapter();
            }
            this.adapter = adapter;
            this.serializer = serializer ?? new Scritchy.Infrastructure.Implementations.EventStorage.Serializers.JsonSerializer();
        }

        Dictionary<object,int> EnumeratorContexts = new Dictionary<object,int>();
        object GlobalEnumeratorContext = new object();

        public IEnumerable<object> GetNewEvents(object Instance = null, object EnumeratorContext = null)
        {
            if (EnumeratorContext == null)
            {
                EnumeratorContext = Instance??GlobalEnumeratorContext;
            }
            if (!EnumeratorContexts.ContainsKey(EnumeratorContext))
                EnumeratorContexts.Add(EnumeratorContext,0);
            var idfrom = EnumeratorContexts[EnumeratorContext];
            IEnumerable<EventBlob> q;
            var ar = Instance as Scritchy.Domain.AR;
            if (ar==null)
            {
                q = adapter.FindAll(idfrom);
            }
            else
            {
                var id = ar.Id;
                var n = Instance.GetType().Name+"Id";
                q = adapter.FindAllWithHeader(n,id,idfrom);
            }
            foreach (EventBlob e in q)
            {
                var t = Type.GetType(e.TypeFullName);
                var obj = serializer.Deserialize(e);
                EnumeratorContexts[EnumeratorContext] += 1;
                yield return obj;
            }
        }

        public bool SaveEvents(IEnumerable<object> events)
        {
            foreach (var evt in events)
            {
                var evtblob = new EventBlob {
                    TypeFullName = evt.GetType().FullName,
                    TypeName = evt.GetType().Name,
                };
                serializer.Serialize(evt, ref evtblob);
                var headers = new List<EventHeader>();
                foreach(var p in evt.GetType().GetProperties().Where(x=>x.Name.EndsWith("Id")))
                {
                    var val = p.GetValue(evt,null);
                    if (val==null)
                        continue;
                    var hdr = new EventHeader{
                        Name = p.Name,
                        Value = val.ToString()
                    };
                    headers.Add(hdr);
                }
                adapter.SaveEvent(evtblob,headers);
            }
            return true;
        }
    }


}
