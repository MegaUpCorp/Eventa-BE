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

        public Task<int> CountAsync(Expression<Func<Participant, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Participant?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Participant?> GetAsync(Expression<Func<Participant, bool>> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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

        public async Task<List<Participant>> GetParticipantsOfEvent (string slug)
        {
            var eventItem = await _eventDAO.GetBySlug(slug);
            if (eventItem == null)
            {
                return new List<Participant>();
            }
            return await _participantDAO.GetByEventIdAsync(eventItem.Id);
        }
    }
}
