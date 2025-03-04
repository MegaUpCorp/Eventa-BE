using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class AccountDTO
    {
        [BsonElement("id")]
        public Guid Id { get; set; }
        [Required]
        [BsonElement("username")]
        public required string Username { get; set; }
        [BsonElement("profilepicture")]
        public string? ProfilePicture { get; set; }
    }
}
