using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;

        public ParticipantService(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task<List<Participant>> GetParticipantsByEventId(Guid eventId)
        {
            return await _participantRepository.GetByEventIdAsync(eventId);
        }

        public async Task<bool> RegisterParticipant(Guid accountId, Guid eventId)
        {
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                EventId = eventId,
            };

            return await _participantRepository.AddAsync(participant);
        }

        public async Task<bool> RemoveParticipant(Guid participantId)
        {
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null)
            {
                return false;
            }
            return await _participantRepository.DeleteAsync(participant);
        }
    }
}
