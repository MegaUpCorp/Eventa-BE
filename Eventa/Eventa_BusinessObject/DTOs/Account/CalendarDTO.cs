using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class CalendarDTO
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

        [BsonElement("location")]
        public LocationDTO? Location { get; set; }
    }

    public class LocationDTO
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
