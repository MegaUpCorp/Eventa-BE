using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class EventCategoryDAO : BaseDAO<EventCategory>
    {
        public EventCategoryDAO(IMongoDatabase database) : base(database, "EventCategories") { }

        public async Task<EventCategory?> GetEventCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetAsync(id, cancellationToken);
        }

        public async Task<bool> AddEventCategoryAsync(EventCategory eventCategoryToAdd, CancellationToken cancellationToken = default)
        {
            return await AddAsync(eventCategoryToAdd, cancellationToken);
        }

        public async Task<bool> UpdateEventCategoryAsync(EventCategory eventCategoryToUpdate, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(eventCategoryToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteEventCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(id, cancellationToken);
        }
    }
}
