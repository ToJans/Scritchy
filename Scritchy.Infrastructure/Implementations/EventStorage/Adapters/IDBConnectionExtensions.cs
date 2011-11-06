using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scritchy.Infrastructure.Implementations.EventStorage.Models;
using System.Data;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Adapters
{
    static class IDBConnectionExtensions
    {
        public static IEnumerable<EventBlob> AllBlobs(this IDbConnection conn, int fromId = 0)
        {
            return conn.GetReader("Select [Id],[SerializedData],[SerializationProtocol],[TypeName],[TypeFullName]"+
                                  " from [EventBlobs] where [Id]>@p0",fromId).Select(x=>x.ToEventBlob());
        }

        public static IEnumerable<EventBlob> AllBlobsWithHeaderNameAndValue(this IDbConnection conn, string name,string value,int fromId = 0)
        {
            return conn.GetReader("Select [Id],[SerializedData],[SerializationProtocol],[TypeName],[TypeFullName] from [EventBlobs] "+
                    "where [Id]>@p0 and exists "+
                        "(select 1 from [EventHeaders]" +
                        "where [EventId] = [Id]" +
                        " and [Name]= @p1"+
                        " and [Value]= @p2)"
                    ,fromId, name,value).Select(x=>x.ToEventBlob());
        }

        public static EventBlob Insert(this IDbConnection conn, EventBlob input)
        {
            return conn.GetReader("insert into [EventBlobs] " +
                "([SerializedData],[SerializationProtocol],[TypeName],[TypeFullName]) values (@p0,@p1,@p2,@p3);" +
                " select [Id],[SerializedData],[SerializationProtocol],[TypeName],[TypeFullName]"+
                " from [EventBlobs] where [Id] = last_insert_rowid();",
                input.SerializedData, input.SerializationProtocol, input.TypeName, input.TypeFullName).Select(x => x.ToEventBlob()).First();
        }

        public static EventHeader Insert(this IDbConnection conn, EventHeader input)
        {
            return conn.GetReader("insert into [EventHeaders] " +
                "([EventId],[Name],[Value]) values (@p0,@p1,@p2);" +
                " select [EventId],[Name],[Value] from [EventHeaders] where [EventId]=@p0 and [Name]=@p1;",
                input.EventId, input.Name, input.Value).Select(x=>x.ToEventHeader()).First();
        }

        static EventBlob ToEventBlob(this IDataReader dr)
        {
            var bl = new EventBlob();
            bl.Id = dr.GetInt32(0);
            bl.SerializedData = dr.GetString(1);
            bl.SerializationProtocol = dr.GetString(2);
            bl.TypeName = dr.GetString(3);
            bl.TypeFullName = dr.GetString(4);
            return bl;
        }

        static EventHeader ToEventHeader(this IDataReader dr)
        {
            var bl = new EventHeader();
            bl.EventId = dr.GetInt32(0);
            bl.Name = dr.GetString(1);
            bl.Value = dr.GetString(2);
            return bl;
        }

        static IEnumerable<IDataReader> GetReader(this IDbConnection conn, string sql, params object[] parametervalues)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                for (int i = 0; i < parametervalues.Length;i++ )
                {
                    var par = cmd.CreateParameter();
                    par.ParameterName = "p" + i.ToString();
                    par.Value = parametervalues[i];
                    cmd.Parameters.Add(par);
                }
                var rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                while (rdr.Read())
                {
                    yield return rdr;
                }
            }

        }



    }
}
