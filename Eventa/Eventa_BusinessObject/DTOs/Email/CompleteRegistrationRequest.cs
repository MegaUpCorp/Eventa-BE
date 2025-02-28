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
        public string Token { get; set; }
        public string UserName { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

    }
}
