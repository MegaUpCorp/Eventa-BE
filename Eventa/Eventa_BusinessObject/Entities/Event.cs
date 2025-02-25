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
    public class Event : BaseEntity
    {
        [Required]
        public string CalendarId { get; set; }

        [Required]
        public string Visibility { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        public Location Location { get; set; }

        public string MeetUrl { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsFree { get; set; } = true;

        [Required]
        public bool RequiresApproval { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public string ProfilePicture { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.String)]
        public Guid OrganizerId { get; set; }
    }

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