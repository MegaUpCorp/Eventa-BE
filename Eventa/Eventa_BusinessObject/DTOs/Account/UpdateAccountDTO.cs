using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class UpdateAccountDTO
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
}
