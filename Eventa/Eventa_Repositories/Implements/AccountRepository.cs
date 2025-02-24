using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
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

        public async Task<List<Calendar>> GetCalendarsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _calendarDAO.GetByAccountIdAsync(accountId, cancellationToken);
        }
    
    }
}
