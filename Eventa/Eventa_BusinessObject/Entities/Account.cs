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
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [BsonElement("password")]
        public string Password { get; set; }
        [StringLength(25)]
        [BsonElement("rolename")]
        public string RoleName { get; set; }
        [BsonElement("fullname")]
        public string? FullName { get; set; }
        [BsonElement("profilepicture")]
        public string? ProfilePicture { get; set; }
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }
        [BsonElement("bio")]
        public string? Bio { get; set; }
        [BsonElement("type")]
        public string? Type { get; set; }
        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }
        [BsonElement("premium")]
        public bool premium { get; set; } 



    }
}
