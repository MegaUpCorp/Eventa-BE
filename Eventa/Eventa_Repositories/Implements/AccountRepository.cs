using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;
        private readonly CalendarDAO _calendarDAO;


        public AccountRepository(AccountDAO accountDAO, CalendarDAO
            calendarDAO)
        {
            _accountDAO = accountDAO;
            _calendarDAO = calendarDAO;
        }

        public async Task<bool> AddAsync(Account entity, CancellationToken cancellationToken = default)
        {
            return await _accountDAO.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Account> entities, CancellationToken cancellationToken = default)
        {
            await _accountDAO.AddRangeAsync(entities, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Account, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return await _accountDAO.CountAsync(filter, cancellationToken);
        }

        public async Task<bool> Delete(params Account[] entities)
        {
            bool isSuccess = true;
            foreach (var entity in entities)
            {
                var success = await _accountDAO.DeleteAsync(entity.Id);
                if (!success)
                {
                    isSuccess = false;
                }
            }
            return isSuccess;
        }

        public async Task<List<Account>> GetAllAsync(Expression<Func<Account, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return await _accountDAO.GetAllAsync(filter, cancellationToken);
        }

        public async Task<Account?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return await _accountDAO.GetAsync(id, cancellationToken);
        }

        public async Task<Account?> GetAsync(Expression<Func<Account, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _accountDAO.GetAsync(filter, cancellationToken);
        }

        public async Task<bool> Update(Account entity)
        {
            return await _accountDAO.UpdateAsync(entity);
        }

        public async Task<bool> UpdateRange(IEnumerable<Account> entities)
        {
            return await _accountDAO.UpdateRangeAsync(entities);
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountDAO.GetAsync(a => a.Email == email);
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return await _accountDAO.GetAsync(a => a.Username == username);
        }

        public async Task<Account?> GetAccountByPhoneNumberAsync(string phoneNumber)
        {
            return await _accountDAO.GetAsync(a => a.PhoneNumber == phoneNumber);
        }
        public async Task<bool> DeleteAsync(params Account[] entities)
        {
            var ids = entities.Select(a => a.Id).ToList();
            return await _accountDAO.DeleteManyAsync(a => ids.Contains(a.Id));

        }
        public async Task<bool> AddCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.AddAsync(calendar, cancellationToken);
        }

        public async Task<List<Calendar>> GetAllCalendarsAsync(CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.GetAllAsync(null, cancellationToken);
        }
        public async Task<Calendar?> GetCalendarByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.GetAsync(id, cancellationToken);
        }
        public async Task<Calendar?> GetCalendarByPublicUrlAsync(string publicUrl)
        {
            return await _calendarDAO.GetAsync(c => c.PublicUrl == publicUrl);
        }
        public async Task<AccountDTO> GetBasicAccountByOrganizerId(Guid accountID, CancellationToken cancellationToken = default)
        {
            var account = await _accountDAO.GetAsync(accountID, cancellationToken);
            if (account == null)
            {
                throw new Exception("Account not found");
            }
            return new AccountDTO
            {
                Id = account.Id,
                Username = account.Username,
                ProfilePicture = account.ProfilePicture
            };
        }
        public async Task<List<Calendar>> GetCalendarsByAccountID(Guid accountID, CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.GetAllAsync(c => c.AccountId == accountID, cancellationToken);
        }
        //public async Task<Calendar?> GetCarlendarByPublicUrl(string publicUrl, CancellationToken cancellationToken = default)
        //{
        //    var calendars = await _calendarDAO.GetAllAsync(c => c.PublicUrl.Equals(publicUrl), cancellationToken);
        //    return calendars.FirstOrDefault();
        //}

        public async Task<bool> SubscribeCalendar(Guid accountId, string url)
        {
            var update = Builders<Calendar>.Update.AddToSet(c => c.SubscribedAccounts, accountId);
            var filter = Builders<Calendar>.Filter.Eq(c => c.PublicUrl, url);
            var result = await _calendarDAO.UpdateOneAsync(c => c.PublicUrl == url, update);
            return result.ModifiedCount > 0;
        }
        public async Task<List<Calendar>> GetCalendarsNotMe(Guid accountID, CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.GetAllAsync(c => c.AccountId != accountID, cancellationToken);
        }

        public async Task<bool> UpdateCalendar(Calendar calendar)
        {
            return await _calendarDAO.UpdateAsync(calendar);
        }

        public async Task<CalendarDTO?> GetCalendarByPublicUrlAsync1(string publicUrl, Guid accountId)
        {
            var calendar = await _calendarDAO.GetAsync(c => c.PublicUrl == publicUrl);
            if (calendar == null)
            {
                return null;
            }
            return new CalendarDTO
            {
                Name = calendar.Name,
                Description = calendar.Description,
                PublicUrl = calendar.PublicUrl,
                ProfilePicture = calendar.ProfilePicture,
                CoverPicture = calendar.CoverPicture,
                Color = calendar.Color,
                CalendarType = calendar.CalendarType,
                Location = calendar.Location != null ? new LocationDTO
                {
                    Id = calendar.Location.Id,
                    Name = calendar.Location.Name,
                    Address = calendar.Location.Address,
                    Latitude = calendar.Location.Latitude,
                    Longitude = calendar.Location.Longitude
                } : null,
                AccountId = calendar.AccountId.ToString(),
                IsSubscribe = calendar.SubscribedAccounts.Contains(accountId)
            };
        }

       public async Task<bool> CheckPremium(Guid accountId)
        {
            var account = await _accountDAO.GetAsync(a => a.Id == accountId);
            if (account == null)
            {
                throw new Exception("Account not found");
            }
            if (account.premium)
            {
                Console.WriteLine("true"); 
                return true;
            }
            return false;
        }

        public Task<bool> UpdatePremium(Guid accountID)
        {
            throw new NotImplementedException();
        }
    }

}
