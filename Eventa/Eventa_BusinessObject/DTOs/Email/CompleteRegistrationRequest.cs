using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Email
{
    public class CompleteRegistrationRequest
    {
        public required string Token { get; set; }
        public required string UserName { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }

    }
}
