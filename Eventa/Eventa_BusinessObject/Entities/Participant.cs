using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities
{
    public class Participant : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("eventID")]
        public Guid EventId { get; set; }
        [BsonRepresentation(BsonType.String)]
        [BsonElement("accountID")]
        public Guid AccountId { get; set; }
        [BsonElement("isConfirmed")]
        public bool IsConfirmed { get; set; }
        [BsonElement("IsCheckedIn")]
        public bool IsCheckedIn { get; set; }


    }
}
