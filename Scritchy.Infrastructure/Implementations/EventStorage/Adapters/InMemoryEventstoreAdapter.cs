using System.Collections.Generic;
using System.Linq;
using Scritchy.Infrastructure.Implementations.EventStorage.Models;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Adapters
{
    public class InMemoryEventstoreAdapter : IEventstoreAdapter
    {
        List<EventBlob> Blobs = new List<EventBlob>();
        List<EventHeader> Headers = new List<EventHeader>();
        static object lockobj = new object();



        public bool SaveEvent(EventBlob blob, IEnumerable<EventHeader> headers)
        {
            lock (lockobj)
            {
                blob.Id = Blobs.Count + 1;
                Blobs.Add(blob);
                foreach (var h in headers)
                {
                    h.EventId = blob.Id;
                    Headers.Add(h);
                }
            }
            return true;
        }

        public IEnumerable<EventBlob> FindAll(long fromid = 0)
        {
            return Blobs.Skip((int)fromid);
        }

        public IEnumerable<EventBlob> FindAllWithHeader(string name, string value, long fromid = 0)
        {
            foreach (var bl in FindAll(fromid))
            {
                if (Headers.Any(x => x.EventId == bl.Id && x.Name == name && x.Value == value))
                    yield return bl;
            }
        }
    }
}
