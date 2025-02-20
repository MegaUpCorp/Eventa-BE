using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Email;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Đăng ký tài khoản sau khi xác thực email thành công
        /// </summary>
        Task<Account> Register(string email, CompleteRegistrationRequest request);

        /// <summary>
        /// Lấy thông tin tài khoản theo email
        /// </summary>
        Task<Account?> GetAccountByEmail(string email);

        /// <summary>
        /// Kiểm tra email đã tồn tại hay chưa
        /// </summary>
        Task<bool> IsEmailExists(string email);
        Task<ActionResult<Account>> GetAccountByAccountId(Guid accountId);
        Task<bool> UpdateAccountById(Guid accountId, UpdateAccountDTO updateAccountDTO, HttpContext httpContext);
        Task<bool> DeleteAccountById(Guid accountId, HttpContext httpContext);

    }
}
