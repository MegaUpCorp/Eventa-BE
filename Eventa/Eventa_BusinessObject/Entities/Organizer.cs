using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Organizer : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("accountId")]
        public Guid AccountId { get; set; }
        [Required]
        [BsonElement("organizerName")]
        public required string OrganizerName { get; set; }
        [BsonElement("organizerDescription")]
        public string? OrganizerDescription { get; set; }

    }
}
