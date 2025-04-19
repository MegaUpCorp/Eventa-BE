using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        Task<Participant?> GetByAccountIdAsync(Guid accountId);
        Task<List<Participant>> GetByEventIdAsync(Guid eventId);
        Task<List<Participant>> GetParticipantsOfEvent(string slug);
        Task<List<Guid>> GetAllEventParticipantedOfMe(Guid accountId);
    }
}
