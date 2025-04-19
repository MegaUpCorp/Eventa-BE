using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
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
        Task<bool> RegisterParticipant(HttpContext httpContext, Guid eventId);
        Task<bool> RemoveParticipant(Guid participantId);
        Task<List<AccountDTO1>> GetParticipantsOfEvent(string slug);
        Task<List<Guid>> GetAllEventParticipantedOfMe(HttpContext httpContext);
    }
}
