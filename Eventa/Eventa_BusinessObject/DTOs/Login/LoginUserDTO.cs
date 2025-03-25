using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Login
{
    public class LoginUserDTO
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public string Picture { get; set; }
    }
}
