using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Models
{
    public class EventBlob
    {
        public int Id { get; set; }
        public string SerializedData { get; set; }
        public string SerializationProtocol {get;set;}
        public string TypeName { get; set; }
        public string TypeFullName { get; set; }
    }

    public class EventHeader
    {
        public int EventId { get; set; }
        public string Name {get;set;}
        public string Value {get;set;}
    }
}
