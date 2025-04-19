using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ParticipantDAO _participantDAO;
        private readonly IEventRepository _eventDAO;

        public ParticipantRepository(ParticipantDAO participantDAO, IEventRepository eventDAO)
        {
            _participantDAO = participantDAO;
            _eventDAO = eventDAO;
        }

        public async Task<bool> AddAsync(Participant entity, CancellationToken cancellationToken = default)
        {
            return await _participantDAO.AddAsync(entity, cancellationToken);
        }

        public async Task<List<Participant>> GetByEventIdAsync(Guid eventId)
        {
            return await _participantDAO.GetByEventIdAsync(eventId);
        }

        public async Task<Participant?> GetByAccountIdAsync(Guid accountId)
        {
            return await _participantDAO.GetByAccountIdAsync(accountId);
        }

        public async Task<bool> DeleteAsync(Participant entity)
        {
            return await _participantDAO.DeleteAsync(entity.Id);
        }

        public async Task<bool> Update(Participant entity)
        {
            return await _participantDAO.UpdateAsync(entity);
        }

        public async Task<Participant?> GetAsync(Expression<Func<Participant, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _participantDAO.GetAsync(filter, cancellationToken);
        }

        // Các phương thức khác chưa cần triển khai ngay
        public Task<int> CountAsync(Expression<Func<Participant, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Participant?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return await _participantDAO.GetAsync(id, cancellationToken);
        }

        public Task<List<Participant>> GetAllAsync(Expression<Func<Participant, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Participant> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(IEnumerable<Participant> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(params Participant[] entities)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Participant>> GetParticipantsOfEvent(string slug)
        {
            var eventItem = await _eventDAO.GetBySlug(slug);
            if (eventItem == null)
            {
                return new List<Participant>();
            }
            return await _participantDAO.GetByEventIdAsync(eventItem.Id);
        }
        public async Task<List<Guid>> GetAllEventParticipantedOfMe(Guid accountId)
        {
            var participants = await _participantDAO.GetAllAsync(p => p.AccountId == accountId);
            return participants.Select(p => p.EventId).Distinct().ToList();
        }
       public async Task<bool> CheckAccountInParticipants(Guid accountId, List<Guid> participantIds)
        {
            var participants = await _participantDAO.GetAllAsync(p => p.AccountId == accountId && participantIds.Contains(p.EventId));
            return participants.Count > 0;
        }
       public async Task<bool> CheckAccountInEvent(Guid accountId, Guid eventId)
        {
            var participants = await _participantDAO.GetAllAsync(p => p.AccountId == accountId && p.EventId == eventId);
            return participants.Count > 0;
        }
    }
}
