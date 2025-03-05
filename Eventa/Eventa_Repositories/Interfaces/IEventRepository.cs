using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAll();
        Task<Event> GetById(Guid id);
        Task AddEvent(Event eventItem);
        Task<bool> UpdateEvent(Guid id, Event eventItem);
        Task RemoveEvent(Guid id);
        Task<Event> GetBySlug( string slug);
    }
}
