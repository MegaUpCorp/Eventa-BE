using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Implements;
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
        private readonly IAccountRepository _accountRepository;

        public ParticipantService(IParticipantRepository participantRepository, IAccountRepository accountRepository)
        {
            _participantRepository = participantRepository;
            _accountRepository = accountRepository;
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
        public async Task<List<AccountDTO1>> GetParticipantsOfEvent(string slug)
        {
            var participants = await _participantRepository.GetParticipantsOfEvent(slug);
            var accountIds = participants.Select(p => p.AccountId).Distinct().ToList();
            var accounts = await _accountRepository.GetAllAsync(a => accountIds.Contains(a.Id));
            return accounts.Select(a => new AccountDTO1
            {
                Id = a.Id,
                Email = a.Email,
               // Username = a.Username,
                FullName = a.FullName,
                ProfilePicture = a.ProfilePicture
            }).ToList();
        }
    }
}
