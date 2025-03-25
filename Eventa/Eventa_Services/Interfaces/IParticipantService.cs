using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IParticipantService
    {
        Task<List<Participant>> GetParticipantsByEventId(Guid eventId);
        Task<bool> RegisterParticipant(Guid accountId, Guid eventId);
        Task<bool> RemoveParticipant(Guid participantId);
        Task<List<Participant>> GetParticipantsOfEvent(string slug);
    }
}
