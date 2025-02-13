using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Login
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(64, ErrorMessage = "Password's max length is 64 characters")]
        public required string Password { get; set; }
    }
}
