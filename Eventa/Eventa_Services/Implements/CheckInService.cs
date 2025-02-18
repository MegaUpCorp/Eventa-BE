using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IEventRepository _eventRepository;

        public CheckInService(ICheckInRepository checkInRepository, IParticipantRepository participantRepository,
                              IEventRepository eventRepository)
        {
            _checkInRepository = checkInRepository;
            _participantRepository = participantRepository;
            _eventRepository = eventRepository;
        }

        public async Task<bool> CheckInParticipant(Guid participantId, Guid eventId)
        {
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null) return false;

            var eventItem = await _eventRepository.GetById(eventId);
            if (eventItem == null) return false;

            var existingCheckIn = await _checkInRepository.GetByParticipantAndEventAsync(participantId, eventId);
            if (existingCheckIn != null) return false; // Already checked in

            var newCheckIn = new CheckIn
            {
                ParticipantId = participantId,
                EventId = eventId,
                CheckInTime = DateTime.UtcNow
            };

            return await _checkInRepository.AddAsync(newCheckIn);
        }
    }
}
