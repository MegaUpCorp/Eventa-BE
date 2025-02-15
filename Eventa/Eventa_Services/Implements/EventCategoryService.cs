using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly EventCategoryDAO _eventCategoryDAO;

        public EventCategoryService(IMongoDatabase database)
        {
            _eventCategoryDAO = new EventCategoryDAO(database);
        }
        public async Task<List<EventCategory>> GetAllEventCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _eventCategoryDAO.GetAllAsync(null, cancellationToken);
        }

        public async Task<EventCategory?> GetEventCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _eventCategoryDAO.GetEventCategoryByIdAsync(id, cancellationToken);
        }

        public async Task<bool> AddEventCategoryAsync(EventCategory eventCategoryToAdd, CancellationToken cancellationToken = default)
        {
            return await _eventCategoryDAO.AddEventCategoryAsync(eventCategoryToAdd, cancellationToken);
        }

        public async Task<bool> UpdateEventCategoryAsync(EventCategory eventCategoryToUpdate, CancellationToken cancellationToken = default)
        {
            return await _eventCategoryDAO.UpdateEventCategoryAsync(eventCategoryToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteEventCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _eventCategoryDAO.DeleteEventCategoryAsync(id, cancellationToken);
        }
    }
}
