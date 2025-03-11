using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Email;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject.Entities;
using Eventa_BusinessObject.Enums;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Eventa_Services.Util;
using Microsoft.AspNetCore.Http;
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
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                ProfilePicture = request.ProfilePicture,
                Password = request.Password,
                RoleName = RoleEnum.Member.ToString()
            };
            var carlandar = new Calendar
            {
                Id = Guid.NewGuid(),
                Name = "Personal",
                Description = "",
                PublicUrl = GenerateRandomString(10),
                ProfilePicture = "",
                CoverPicture = "",
                Color = "",
                CalendarType = "Created",
                AccountId = account.Id,
                Location = new Location
                {
                    Id = "",
                    Name = "",
                    Address = "",
                    Latitude = 0,
                    Longitude = 0
                }
            };
            await _accountRepository.AddCalendarAsync(carlandar);
            await _accountRepository.AddAsync(account);
            return account;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<string> AddCalendarToAccount(CalendarDTO calendar,HttpContext httpContext)
        {
            var AccountId = UserUtil.GetAccountId(httpContext).Value;
            // Check if the PublicUrl already exists in the database
            var existingCalendar = await _accountRepository.GetCalendarByPublicUrlAsync(calendar.PublicUrl);
            if (existingCalendar != null)
            {
                return "Public URL already exists";
            }

            var calendarEntity = new Calendar
            {
                Name = calendar.Name,
                Description = calendar.Description,
                PublicUrl = calendar.PublicUrl,
                ProfilePicture = calendar.ProfilePicture,
                CoverPicture = calendar.CoverPicture,
                Color = calendar.Color,
                CalendarType = calendar.CalendarType,
                AccountId = AccountId,
                SubscribedAccounts = new List<Guid>(),
                Location = calendar.Location != null ? new Location
                {
                    Id = calendar.Location.Id,
                    Name = calendar.Location.Name,
                    Address = calendar.Location.Address,
                    Latitude = calendar.Location.Latitude,
                    Longitude = calendar.Location.Longitude
                } : null
            };

            var result = await _accountRepository.AddCalendarAsync(calendarEntity);
            return result ? "Calendar added successfully" : "Failed to add calendar";
        }

        public async Task<List<Calendar>> GetAllCalendarsAsync()
        {
            var calendars = await _accountRepository.GetAllCalendarsAsync();
            return calendars.ToList();
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
            var account = await _accountRepository.GetAsync(accountId);
            if (account == null)
            {
                return new NotFoundObjectResult($"Account with ID {accountId} not found.");
            }
            return account;
        }
        public async Task<bool> UpdateAccountById(Guid accountId, UpdateAccountDTO updateAccountDTO, HttpContext httpContext)
        {
            var roleName = UserUtil.GetRoleName(httpContext);
            if (roleName != RoleEnum.Admin.ToString() && roleName != RoleEnum.Member.ToString())
            {
                return false;
            }

            var account = await _accountRepository.GetAsync(accountId);
            if (account == null)
            {
                return false;
            }
            var avatarUrl = updateAccountDTO.ProfilePicture != null
                ? await _firebaseService.UploadFile(updateAccountDTO.ProfilePicture)
                : account.ProfilePicture;
            account.Username = updateAccountDTO.Username ?? account.Username;
            account.PhoneNumber = updateAccountDTO.PhoneNumber ?? account.PhoneNumber;
            account.ProfilePicture = avatarUrl ?? account.ProfilePicture;
            account.Password = updateAccountDTO.Password ?? account.Password;
            account.Email = updateAccountDTO.Email ?? account.Email;
            account.Address = updateAccountDTO.Address ?? account.Address;
            account.Bio = updateAccountDTO.Bio ?? account.Bio;
            var updated = await _accountRepository.Update(account);
            return updated;
        }
        public async Task<bool> DeleteAccountById(Guid accountId, HttpContext httpContext)
        {
            var roleName = UserUtil.GetRoleName(httpContext);
            if(roleName != RoleEnum.Admin.ToString())
            {
                return false;
            }
            var account = await _accountRepository.GetAsync(accountId);
            if (account == null)
            {
                return false;
            }
            var deleted = await _accountRepository.DeleteAsync(account);
            return deleted;
        }
       public async Task<List<CarlenderReponse>> GetCarlendersByAccountID(HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            var calendars = await _accountRepository.GetCalendarsByAccountID(accountID.Value);
            return calendars.Select(c => new CarlenderReponse
            {
                Id = c.Id,
                Name = c.Name,
                ProfilePicture = c.ProfilePicture,
                PublicUrl = c.PublicUrl,
                SubscribedAccounts = c.SubscribedAccounts,
            }).ToList();

        }
        public async Task<CalendarDTO?> GetCarlendarByPublicUrl(string publicUrl)
        {
            var carlandar = await _accountRepository.GetCalendarByPublicUrlAsync1(publicUrl);
            return carlandar;
        }
        public async Task<List<Calendar>> GetListCarlandarNotMe(HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            var calendars = await _accountRepository.GetCalendarsNotMe(accountID.Value);
            return calendars.ToList();
        }

        public async Task<bool> SubscribeCalendar(string publicUrl, HttpContext httpContext)
        {
            var accountId = UserUtil.GetAccountId(httpContext).Value;
            var calendar = await _accountRepository.GetCalendarByPublicUrlAsync(publicUrl);
            if( calendar.AccountId == accountId)
            {
                return false;
            }

            return await _accountRepository.SubscribeCalendar(accountId, publicUrl);
        }
        public async Task<bool> UnsubscribeCalendar(string publicUrl, HttpContext httpContext)
        {
            var accountId = UserUtil.GetAccountId(httpContext).Value;
            var calendar = await _accountRepository.GetCalendarByPublicUrlAsync(publicUrl);
            if (calendar == null || !calendar.SubscribedAccounts.Contains(accountId))
            {
                return false;
            }

            calendar.SubscribedAccounts.Remove(accountId);
            return await _accountRepository.UpdateCalendar(calendar);
        }


        public async Task<List<Calendar>> GetCalendarsUserSubcribed(HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            if (accountID == null)
            {
                return new List<Calendar>();
            }

            var calendars = await _accountRepository.GetAllCalendarsAsync();
            var subscribedCalendars = calendars.Where(c => c.SubscribedAccounts.Contains(accountID.Value)).ToList();

            return subscribedCalendars;
        }

    }
}
