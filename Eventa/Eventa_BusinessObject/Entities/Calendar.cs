using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.Entities
{
    public class Calendar : BaseEntity
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
        public Location? Location { get; set; }
        [BsonElement("accountId")]
        [BsonRepresentation(BsonType.String)] // Chỉ định kiểu lưu trữ GUID
        public Guid AccountId { get; set; }

        [BsonElement("subscribedAccounts")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> SubscribedAccounts { get; set; } = new();
    }

    public class Location
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [Required]
        [BsonElement("longitude")]
        public double Longitude { get; set; }
    }
}
