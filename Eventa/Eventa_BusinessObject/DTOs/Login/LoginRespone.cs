﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Login
{
    public class LoginRespone
    {
        public string Token { get; set; }
        public string? ProfilePicture { get; set; }
        public string RoleName { get; set; }
    }
}
