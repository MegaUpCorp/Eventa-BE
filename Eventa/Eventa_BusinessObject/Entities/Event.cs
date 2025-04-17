using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Validations;

namespace Eventa_BusinessObject.Entities
{
    public class Event : BaseEntity
    {
        [BsonRepresentation(BsonType.String)] public Guid CalendarId { get; set; }

        [Required] public string Visibility { get; set; }

        [Required] [MaxLength(200)] public string Title { get; set; }

        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        [Required] public bool IsOnline { get; set; }
    
        public Location Location { get; set; }
        
        [BsonElement("BankAcc")]
        public BankAcc? BankAcc { get; set; }
        public string MeetUrl { get; set; }

        public string Description { get; set; }

        [Required] public bool IsFree { get; set; } = true;

        [Required] public bool RequiresApproval { get; set; }

        [Required] public int Capacity { get; set; }

        [Required] public string Slug { get; set; }

        [Required] public string ProfilePicture { get; set; }

        [Required]
        // [BsonElement("price")]
        public double Price { get; set; }

        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("OrganizerId")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> OrganizerId { get; set; } = new();
    }

    public class BankAcc
    {
        [BsonElement("acc")] public string acc { get; set; } = string.Empty;
        [BsonElement("bank")] public string bank { get; set; } = string.Empty;
        [BsonElement("amount")] public string amount { get; set; } = string.Empty;
        [BsonElement("des")] public string des { get; set; } = string.Empty;
    }
}