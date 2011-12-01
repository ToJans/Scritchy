using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Adapters
{
    public class ShardedAdapter : IEventstoreAdapter
    {
        IEventstoreAdapter[] Shards;
        BackgroundWorker bw = new BackgroundWorker();

        // Last recently used Adapter; must be persisted
        public int LRU {get;set;}

        public ShardedAdapter(params IEventstoreAdapter[] Shards)
        {
            this.Shards = Shards;
        }

        public bool SaveEvent(Models.EventBlob blob, IEnumerable<Models.EventHeader> Headers)
        {
            var res = Shards[LRU].SaveEvent(blob, Headers);
            LRU++;
            LRU %= Shards.Length;
            return res;
        }

        public IEnumerable<Models.EventBlob> FindAll(long fromid = 0)
        {
            var selects = Shards.Select((x,i) => x.FindAll((fromid-i)/Shards.Length).AsEnumerable().GetEnumerator()).ToArray();
            fromid %= Shards.Length;
            while (selects[fromid].MoveNext())
            {
                yield return selects[fromid].Current;
                fromid++;
                fromid %= Shards.Length;
            }
        }

        public IEnumerable<Models.EventBlob> FindAllWithHeader(string name, string value, long fromid = 0)
        {
            var selects = Shards.Select((x, i) => x.FindAll((fromid - i) / Shards.Length).AsEnumerable().GetEnumerator()).ToList();
            var pos = (int)(fromid % selects.Count);
            while (selects.Any())
            {
                if (selects[pos].MoveNext())
                {
                    yield return selects[pos].Current;
                    pos++;
                    pos %= selects.Count;
                }
                else
                {
                    selects.RemoveAt(pos);
                }
            }
        }
    }
}
