using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Account : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("accountId")]
        public string AccountId { get; set; }
        [Required]
        [EmailAddress]
        [BsonElement("email")]
        public required string Email { get; set; }
        [Required]
        [BsonElement("username")]
        public required string Username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [BsonElement("password")]
        public required string Password { get; set; }
        [StringLength(25)]
        [BsonElement("rolename")]
        public required string RoleName { get; set; }
        [BsonElement("profilepicture")]
        public string? ProfilePicture { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        [BsonElement("phoneNumber")]
        public required string PhoneNumber { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }
        [BsonElement("bio")]
        public string? Bio { get; set; }


    }
}
