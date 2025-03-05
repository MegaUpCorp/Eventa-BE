using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly OrganizerDAO _organizerDAO;

        public OrganizerRepository(OrganizerDAO organizerDAO)
        {
            _organizerDAO = organizerDAO;
        }

        public async Task<bool> AddAsync(Organizer entity, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Organizer> entities, CancellationToken cancellationToken = default)
        {
            await _organizerDAO.AddRangeAsync(entities, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Organizer, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
             return await _organizerDAO.CountAsync(filter, cancellationToken);
        }

        public async Task<bool> DeleteAsync(params Organizer[] entities)
        {
            bool isSuccess = true;
            foreach (var entity in entities)
            {
                var success = await _organizerDAO.DeleteAsync(entity.Id);
                if (!success)
                {
                    isSuccess = false;
                }
            }
            return isSuccess;
        }

        public async Task<List<Organizer>> GetAllAsync(Expression<Func<Organizer, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.GetAllAsync(filter, cancellationToken);
        }

        public async Task<Organizer?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.GetAsync(id, cancellationToken);
        }

        public async Task<Organizer?> GetAsync(Expression<Func<Organizer, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.GetAsync(filter, cancellationToken);
        }

        public async Task<Organizer?> GetByAccountIdAsync(Guid accountId)
        {
            return await _organizerDAO.GetByAccountIdAsync(accountId);
        }

        public async Task<bool> Update(Organizer entity)
        {
            return await _organizerDAO.UpdateAsync(entity);
        }

        public async Task<bool> UpdateRange(IEnumerable<Organizer> entities)
        {
            return await _organizerDAO.UpdateRangeAsync(entities);
        }
        public async Task<AccountDTO> GetAccountOfOrganizer(Guid organizerId)
        {
            var account = await _organizerDAO.GetAccountByOrganizerId(organizerId);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found for the given organizer ID.");
            }
            return new AccountDTO
            {
                // Assuming AccountDTO has similar properties to Account
                Id = account.Id,
                Username = account.Username,
                ProfilePicture = account.ProfilePicture,
                // Map other properties as needed
            };
        }
        public async Task<List<Guid>> GetOrganizerIdsByEventId(Guid eventId)
        {
            return await _organizerDAO.GetOrganizerIdsByEventId(eventId);
        }
       public async Task<bool> CheckAccountInOrganizers(Guid accountId, List<Guid> organizerIds)
        {
            return await _organizerDAO.CheckAccountInOrganizers(accountId, organizerIds);
        }
    }
}
