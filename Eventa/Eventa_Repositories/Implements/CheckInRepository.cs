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
    public class CheckInRepository : ICheckInRepository
    {
        private readonly CheckInDAO _checkInDAO;

        public CheckInRepository(CheckInDAO checkInDAO)
        {
            _checkInDAO = checkInDAO;
        }

        public async Task<bool> AddAsync(CheckIn entity, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.AddAsync(entity, cancellationToken);
        }

        public Task AddRangeAsync(IEnumerable<CheckIn> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<CheckIn, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(params CheckIn[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<List<CheckIn>> GetAllAsync(Expression<Func<CheckIn, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CheckIn?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CheckIn?> GetAsync(Expression<Func<CheckIn, bool>> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<CheckIn?> GetByParticipantAndEventAsync(Guid participantId, Guid eventId)
        {
            return await _checkInDAO.GetByParticipantAndEventAsync(participantId, eventId);
        }

        public Task<bool> Update(CheckIn entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(IEnumerable<CheckIn> entities)
        {
            throw new NotImplementedException();
        }
    }
}
