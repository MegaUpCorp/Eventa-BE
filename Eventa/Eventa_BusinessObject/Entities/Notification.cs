using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Notification : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("accountId")]
        public Guid AccountId { get; set; }
        [BsonElement("message")]
        public string? Message { get; set; }
        [BsonElement("isRead")]
        public bool IsRead { get; set; }
    }
}
