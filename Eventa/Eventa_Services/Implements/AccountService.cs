using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject.Entities;
using Eventa_BusinessObject.Enums;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
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

        public async Task<bool> Register(CreateAccountRequest createAccountRequest)
        {
            var url = "";
            if (createAccountRequest.AvatarImage != null)
            {
                url = await _firebaseService.UploadFile(createAccountRequest.AvatarImage);

            }
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Email = createAccountRequest.Email,
                Username = createAccountRequest.UserName,
                PhoneNumber = createAccountRequest.PhoneNumber,
                ProfilePicture = url,
                Password = createAccountRequest.Password,
                RoleName = RoleEnum.Member.ToString()
            };
            await _accountRepository.AddAsync(account);
            return true;
        }
    }
}
