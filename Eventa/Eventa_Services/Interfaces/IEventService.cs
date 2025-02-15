using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public  interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
        Task<Event?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AddEventAsync(Event eventToAdd, CancellationToken cancellationToken = default);
        Task<bool> UpdateEventAsync(Event eventToUpdate, CancellationToken cancellationToken = default);
        Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
