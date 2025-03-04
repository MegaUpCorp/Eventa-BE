using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class AccountResponeGetMe
    {
        public Guid id { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public string? phoneNumber { get; set; }
        public string roleName { get; set; }
        public string? profilePicture { get; set; }
        public string? address { get; set; }
        public string? bio { get; set; }
        public string? type { get; set; }
        public DateTime insDate { get; set; }
        public DateTime updDate { get; set; }   
    }
}
