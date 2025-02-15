using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class CheckInDAO : BaseDAO<CheckIn>
    {
        public CheckInDAO(IMongoDatabase database) : base(database, "CheckIns") { }

        public async Task<CheckIn?> GetCheckInByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetAsync(id, cancellationToken);
        }

        public async Task<bool> AddCheckInAsync(CheckIn checkInToAdd, CancellationToken cancellationToken = default)
        {
            return await AddAsync(checkInToAdd, cancellationToken);
        }

        public async Task<bool> UpdateCheckInAsync(CheckIn checkInToUpdate, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(checkInToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteCheckInAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(id, cancellationToken);
        }

        public async Task<List<CheckIn>> GetCheckInsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            Expression<Func<CheckIn, bool>> filter = c => c.EventId == eventId;
            return await GetAllAsync(filter, cancellationToken);
        }
        public async Task<List<CheckIn>> GetCheckInsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            Expression<Func<CheckIn, bool>> filter = c => c.UserId == userId;
            return await GetAllAsync(filter, cancellationToken);
        }
    }
}
