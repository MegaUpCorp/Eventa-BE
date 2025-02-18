using Eventa_BusinessObject.DTOs.Event;
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
    public class EventService : IEventService
    {

        private readonly IEventRepository _eventRepository;
        private readonly IOrganizerRepository _organizerRepository;
        public EventService(IEventRepository eventRepository, IOrganizerRepository organizerRepository)
        {
            _eventRepository = eventRepository;
            _organizerRepository = organizerRepository;
        }

        public async Task<List<Event>> GetAllEvents()
        {
            return await _eventRepository.GetAll();
        }

        public async Task<Event> GetEventById(Guid id)
        {
            return await _eventRepository.GetById(id);
        }

        public async Task<string> AddEvent(CreateEventDTO eventItem, HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            var userName = UserUtil.GetName(httpContext);
            if (accountID == null)
            {
                return "Account not found";
            }

            var organizer = new Organizer
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

            var newEvent = new Event
            {
                Title = eventItem.Title,
                Description = eventItem.Description,
                StartDate = eventItem.StartDate,
                EndDate = eventItem.EndDate,
                Location = eventItem.Location,
                MaxParticipants = eventItem.MaxParticipants,
                TimeZone = eventItem.TimeZone,
                ImageUrl = eventItem.ImageUrl,
                Visibility = eventItem.Visibility,
                Capacity = eventItem.Capacity,
                Blug = eventItem.Blug,
                RequiresApproval = eventItem.RequiresApproval,
                IsFree = eventItem.IsFree,
                CreatedAt = DateTime.UtcNow,
                OrganizerId = organizer.Id
            };

            await _eventRepository.AddEvent(newEvent);
            return "Event created successfully";
        }


        public async Task<bool> UpdateEvent(Guid id, UpdateEventDTO eventUpdateDTO)
        {
            var existingEvent = await _eventRepository.GetById(id);
            if (existingEvent == null)
            {
                return false; 
            }
            existingEvent.Title = eventUpdateDTO.Title ?? existingEvent.Title;
            existingEvent.Description = eventUpdateDTO.Description ?? existingEvent.Description;
            existingEvent.StartDate = eventUpdateDTO.StartDate ?? existingEvent.StartDate;
            existingEvent.EndDate = eventUpdateDTO.EndDate ?? existingEvent.EndDate;
            existingEvent.Location = eventUpdateDTO.Location ?? existingEvent.Location;
            existingEvent.MaxParticipants = eventUpdateDTO.MaxParticipants ?? existingEvent.MaxParticipants;
            existingEvent.TimeZone = eventUpdateDTO.TimeZone ?? existingEvent.TimeZone;
            existingEvent.ImageUrl = eventUpdateDTO.ImageUrl ?? existingEvent.ImageUrl;
            existingEvent.Visibility = eventUpdateDTO.Visibility ?? existingEvent.Visibility;
            existingEvent.RequiresApproval = eventUpdateDTO.RequiresApproval ?? existingEvent.RequiresApproval;
            existingEvent.IsFree = eventUpdateDTO.IsFree ?? existingEvent.IsFree;
            existingEvent.Capacity = eventUpdateDTO.Capacity ?? existingEvent.Capacity;
            existingEvent.Blug = eventUpdateDTO.Blug ?? existingEvent.Blug;

            return await _eventRepository.UpdateEvent(id, existingEvent);
        }


        public async Task<bool> RemoveEvent(Guid id)
        {
            var existingEvent = await _eventRepository.GetById(id);
            if (existingEvent == null)
            {
                return false;
            }
            var organizer = await _organizerRepository.GetAsync(existingEvent.OrganizerId);
            if (organizer == null)
            {
                return false;
            }
            var removedOrganizer = await _organizerRepository.DeleteAsync(organizer);
            await _eventRepository.RemoveEvent(id);
            return true;
        }
    }
}
