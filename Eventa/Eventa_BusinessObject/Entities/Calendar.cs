using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Calendar : BaseEntity
    {
        [Required]
        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("accountId")]
        public Guid AccountId { get; set; }

    }
}
