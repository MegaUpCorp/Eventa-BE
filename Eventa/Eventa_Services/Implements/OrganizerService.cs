using Eventa_BusinessObject.DTOs.Organizer;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Eventa_Services.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRepository _organizerRepository;
        public OrganizerService(IOrganizerRepository organizerRepository)
        {
            _organizerRepository = organizerRepository;
        }

        public async Task<string> AddOrganizer(Organizer organizer, HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            var userName = UserUtil.GetName(httpContext);
            if (accountID == null)
            {
                return "Account not found";
            }

            var newOrganizer = new Organizer  
            {
                Id = Guid.NewGuid(),
                AccountId = accountID.Value,
                OrganizerName = userName
            };

            var organizerAdded = await _organizerRepository.AddAsync(organizer);
            if (!organizerAdded)
            {
                return "Failed to add organizer";
            }

            return "Organizer added successfully";
        }

        public async Task<bool> DeleteOrganizerById(Guid id)
        {
            var existingOrganizer = await _organizerRepository.GetAsync(id);
            if (existingOrganizer == null)
            {
                return false;
            }
            var deleted = await _organizerRepository.DeleteAsync(existingOrganizer);
            return deleted;

        }

        public async Task<ActionResult<List<Organizer>>> GetAllOrganizer()
        {
            return await _organizerRepository.GetAllAsync();
        }

        public async Task<ActionResult<Organizer?>> GetOrganizerByAccountId(Guid accountId)
        {
            var organizer = await _organizerRepository.GetByAccountIdAsync(accountId);
            if (organizer == null)
            {
                return new NotFoundObjectResult($"Organizer with AccountId {accountId} not found.");
            }
            return organizer;

        }

        public async Task<ActionResult<Organizer?>> GetOrganizerById(Guid id)
        {
            var organizer = await _organizerRepository.GetAsync(id);
            if (organizer == null)
            {
                return new NotFoundResult();
            }
            return new ActionResult<Organizer?>(organizer);
        }

        public async Task<bool> UpdateOrganizerById(Guid id,UpdateOrganizerDTO updateOrganizerDTO)
        {
            var existingOrganizer = await _organizerRepository.GetAsync(id);
            if (existingOrganizer == null)
            {
                return false;
            }
            existingOrganizer.OrganizerName = updateOrganizerDTO.OrganizerName ?? existingOrganizer.OrganizerName;
            existingOrganizer.OrganizerDescription = updateOrganizerDTO.OrganizerDescription ?? existingOrganizer.OrganizerDescription;
            var updated = await _organizerRepository.Update(existingOrganizer);
            return updated;
        }
        public async Task<bool> AddOganizerForEvent(Guid accountId, string slug)
        {
            return await _organizerRepository.AddOganizerForEvent(accountId, slug);
        }
    }
}
