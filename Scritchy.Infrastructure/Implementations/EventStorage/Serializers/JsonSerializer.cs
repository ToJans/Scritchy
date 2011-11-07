using System;
using Newtonsoft.Json;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Serializers
{
    public class JsonSerializer: ISerializer
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };


        public void Serialize(object instance, ref Models.EventBlob blob)
        {
            blob.SerializedData = JsonConvert.SerializeObject(instance,Formatting.Indented,settings);
            blob.SerializationProtocol = "JSON.Net";
        }

        public object Deserialize(Models.EventBlob blob)
        {
            Type t = Type.GetType(blob.TypeFullName);
            var obj = JsonConvert.DeserializeObject(blob.SerializedData, t,settings);
            return obj;
        }
    }
}
