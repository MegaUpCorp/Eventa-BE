using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Email;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject.Entities;
using Eventa_BusinessObject.Enums;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IFirebaseService _firebaseService;
        public AccountService(IAccountRepository accountRepository, IFirebaseService firebaseService)
        {
            _accountRepository = accountRepository;
            _firebaseService = firebaseService;
        }

        public async Task<Account> Register(string email, CompleteRegistrationRequest request)
        {
            
            var avatarUrl = request.ProfilePicture != null
        ? await _firebaseService.UploadFile(request.ProfilePicture)
        : "";

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Email = email,
                Username = request.UserName,
                PhoneNumber = request.PhoneNumber,
                ProfilePicture = avatarUrl,
                Password = request.Password,
                RoleName = RoleEnum.Member.ToString()
            };

            await _accountRepository.AddAsync(account);
            return account;
        }
        public async Task<Account?> GetAccountByEmail(string email)
        {
            return await _accountRepository.GetAccountByEmailAsync(email);
        }

        public async Task<bool> IsEmailExists(string email)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            return account != null;
        }
        public async Task<ActionResult<Account?>> GetAccountByAccountId(Guid accountId)
        {
            return await _accountRepository.GetAsync(accountId);
        }
    }
}
