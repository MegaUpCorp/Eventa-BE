using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Event
{
    public class Location
    {
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } // Thêm Id để khớp với JSON

        [Required]
        [BsonElement("name")]
        public string Name { get; set; } // Thêm Name để khớp với JSON

        [Required]
        [BsonElement("address")]
        public string Address { get; set; }

        [Required]
        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [Required]
        [BsonElement("longitude")]
        public double Longitude { get; set; }
    }
}
