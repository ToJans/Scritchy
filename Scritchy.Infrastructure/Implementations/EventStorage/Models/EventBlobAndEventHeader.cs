
namespace Scritchy.Infrastructure.Implementations.EventStorage.Models
{
    [ProtoBuf.ProtoContract(ImplicitFields=ProtoBuf.ImplicitFields.AllPublic)]
    public class EventBlob
    {
        public int Id { get; set; }
        public string SerializedData { get; set; }
        public string SerializationProtocol {get;set;}
        public string TypeName { get; set; }
        public string TypeFullName { get; set; }
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class EventHeader
    {
        public int EventId { get; set; }
        public string Name {get;set;}
        public string Value {get;set;}
    }
}
