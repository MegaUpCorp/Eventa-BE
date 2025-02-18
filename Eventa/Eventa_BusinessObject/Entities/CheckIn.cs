using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class CheckIn : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid ParticipantId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid EventId { get; set; }

        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    }
}
