using Eventa_BusinessObject.DTOs.Account;
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
        Task<bool> UpdateEventBySlug(string slug, Event eventItem);

        Task RemoveEvent(Guid id);
        Task<Event> GetBySlug( string slug);
        Task<List<Guid>> GetOrganizerIdsByEventId(Guid eventId);
        Task<List<AccountDTO>> GetSubscribedAccounts(string slug);
        Task<List<Event>> GetEventsOfMe(Guid accountId);
    }
}
