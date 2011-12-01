using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Scritchy.Infrastructure.Implementations.EventStorage.Adapters
{
    public class StreamAdapter : IEventstoreAdapter
    {
        public enum StreamAccess
        {
            ForReading,ForWriting
        }

        const string GLOBAL = "GLOBAL";
        const string HeaderPrefix = "HEADER";

        Func<string, StreamAccess, Stream> OpenStream;

        class ReaderPosition
        {
            public long index;
            public long position;
        }

        Dictionary<string, Stream> writerstreams = new Dictionary<string, Stream>();
        Dictionary<string, ReaderPosition> ReaderPositions = new Dictionary<string, ReaderPosition>();

        public StreamAdapter(Func<string, StreamAccess, Stream> OpenStream)
        {
            this.OpenStream = OpenStream;
        }

        Stream GetWriterStream(string name)
        {
            if (!writerstreams.ContainsKey(name))
            {
                writerstreams.Add(name, OpenStream(name,StreamAccess.ForWriting));
            }
            return writerstreams[name];
        }

        public bool SaveEvent(Models.EventBlob blob, IEnumerable<Models.EventHeader> Headers)
        {
            ProtoBuf.Serializer.SerializeWithLengthPrefix(GetWriterStream(GLOBAL), blob, ProtoBuf.PrefixStyle.Base128);
            GetWriterStream(GLOBAL).Flush();
            foreach (var h in Headers)
            {
                var str = GetWriterStream(HeaderStreamname(h));
                ProtoBuf.Serializer.SerializeWithLengthPrefix(str, blob, ProtoBuf.PrefixStyle.Base128);
                str.Flush();
            }
            return true;
        }

        string HeaderStreamname(Models.EventHeader h)
        {
            return HeaderPrefix + "_" + h.Name + "_" + h.Value;
        }

        ReaderPosition GetReaderPosition(string name)
        {
            if (!ReaderPositions.ContainsKey(name))
                ReaderPositions.Add(name, new ReaderPosition());
            return ReaderPositions[name];
        }

        public IEnumerable<Models.EventBlob> FindAll(long fromid = 0)
        {
            return ReadAllFromStream<Models.EventBlob>(GLOBAL, fromid);
        }

        public IEnumerable<Models.EventBlob> FindAllWithHeader(string name, string value, long fromid = 0)
        {
            var evb = new Models.EventHeader { Name = name, Value = value };
            var strname = HeaderStreamname(evb);
            return ReadAllFromStream<Models.EventBlob>(strname, fromid);
        }

        IEnumerable<T> ReadAllFromStream<T>(string name,long fromid = 0) where T:class 
        {
            using (var stream = OpenStream(name,StreamAccess.ForReading))
            {
                var pos = GetReaderPosition(name);
                if (pos.index > fromid)
                    pos.index = pos.position = 0;
                bool eos = false;
                while (!eos)
                {
                    T v = null;
                    stream.Seek(pos.position, SeekOrigin.Begin);
                    v = ProtoBuf.Serializer.DeserializeWithLengthPrefix<T>(stream, ProtoBuf.PrefixStyle.Base128);
                    if (v == null)
                    {
                        eos = true;
                    }
                    else
                    {
                        if (pos.index >= fromid)
                            yield return v;
                        pos.index++;
                        pos.position = stream.Position;
                    }
                }
            }
        }
    }
}
