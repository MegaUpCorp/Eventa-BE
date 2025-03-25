using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class AccountDTO1
    {
        [BsonElement("id")]
        public Guid Id { get; set; }
        [Required]
        [BsonElement("fullName")]
        public required string FullName { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("profilepicture")]
        public string? ProfilePicture { get; set; }
    }
}
