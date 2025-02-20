using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Login
{
    public class LoginData
    {
        public LoginData(Guid id, string fullName, string roleCode, string avatar, string email, string type)
        {
            Id = id;
            FullName = fullName;
            RoleCode = roleCode;
            Avatar = avatar;
            Email = email;
            Type = type;
        }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleCode { get; set; }
        public string Avatar { get; set; }
        public string Type { get; set; }
    }
}
