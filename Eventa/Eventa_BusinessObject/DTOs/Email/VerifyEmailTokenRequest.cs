﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Email
{
    public class VerifyEmailTokenRequest
    {
        public required string Token { get; set; }
    }
}
