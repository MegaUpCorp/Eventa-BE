using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class CreateCalendarDTO
    {
        [Required]
        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("publicUrl")]
        public string? PublicUrl { get; set; }

        [BsonElement("profilePicture")]
        public string? ProfilePicture { get; set; }

        [BsonElement("coverPicture")]
        public string? CoverPicture { get; set; }

        [BsonElement("color")]
        public string? Color { get; set; }
        [BsonElement("calendarType")]
        public string? CalendarType { get; set; }

        [BsonElement("location")]
        public LocationDTO3? Location { get; set; }

    }

    public class LocationDTO3
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("lat")]
        public double Latitude { get; set; }

        [BsonElement("lng")]
        public double Longitude { get; set; }
    }
}
