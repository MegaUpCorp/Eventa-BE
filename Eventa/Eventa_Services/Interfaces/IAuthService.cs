﻿using Eventa_BusinessObject.Entities;
using Eventa_Services.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtToken(Account account);
        Task<Account> Authenticate(string email, string password);
        Task<string> GoogleLogin();
        Task<Result<object>> GoogleCallback(string token);
    }
}
