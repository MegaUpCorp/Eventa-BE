using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IEventCategoryService
    {
        Task<List<EventCategory>> GetAllEventCategoriesAsync(CancellationToken cancellationToken = default);

        Task<EventCategory?> GetEventCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AddEventCategoryAsync(EventCategory eventCategoryToAdd, CancellationToken cancellationToken = default);
        Task<bool> UpdateEventCategoryAsync(EventCategory eventCategoryToUpdate, CancellationToken cancellationToken = default);
        Task<bool> DeleteEventCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
