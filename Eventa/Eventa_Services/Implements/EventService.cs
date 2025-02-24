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
            if (eventItem.IsOnline && string.IsNullOrWhiteSpace(eventItem.MeetUrl))
            {
                return "Online event must have a Meet URL.";
            }
            if (!eventItem.IsOnline && (eventItem.Location == null ||
                                        string.IsNullOrWhiteSpace(eventItem.Location.Address)))
            {
                return "Offline event must have a valid location.";
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
                CalendarId = eventItem.CalendarId,
                Visibility = eventItem.Visibility,
                Title = eventItem.Title,
                StartDate = eventItem.StartDate,
                EndDate = eventItem.EndDate,
                IsOnline = eventItem.IsOnline,
                Location = eventItem.IsOnline ? null : new Location
                {
                    Address = eventItem.Location.Address,
                    Latitude = (double)eventItem.Location.Latitude,
                    Longitude = (double)eventItem.Location.Longitude
                },
                MeetUrl = eventItem.IsOnline ? eventItem.MeetUrl : null,
                Description = eventItem.Description,
                IsFree = eventItem.IsFree,
                RequiresApproval = eventItem.RequiresApproval,
                Capacity = eventItem.Capacity,
                Slug = eventItem.Slug,
                ProfilePicture = eventItem.ProfilePicture,
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
            existingEvent.CalendarId = eventUpdateDTO.CalendarId ?? existingEvent.CalendarId;
            existingEvent.Visibility = eventUpdateDTO.Visibility ?? existingEvent.Visibility;
            existingEvent.Title = eventUpdateDTO.Title ?? existingEvent.Title;
            existingEvent.Description = eventUpdateDTO.Description ?? existingEvent.Description;
            existingEvent.StartDate = eventUpdateDTO.StartDate ?? existingEvent.StartDate;
            existingEvent.EndDate = eventUpdateDTO.EndDate ?? existingEvent.EndDate;
            existingEvent.IsOnline = eventUpdateDTO.IsOnline ?? existingEvent.IsOnline;

            if (eventUpdateDTO.IsOnline == false && eventUpdateDTO.Location != null)
            {
                existingEvent.Location = new Location
                {
                    Address = eventUpdateDTO.Location.Address ?? existingEvent.Location?.Address,
                    Latitude = (double)(eventUpdateDTO.Location.Latitude ?? existingEvent.Location?.Latitude),
                    Longitude = (double)(eventUpdateDTO.Location.Longitude ?? existingEvent.Location?.Longitude)
                };
            }
            else if (eventUpdateDTO.IsOnline == true)
            {
                existingEvent.MeetUrl = eventUpdateDTO.MeetUrl ?? existingEvent.MeetUrl;
                existingEvent.Location = null;
            }

            existingEvent.IsFree = eventUpdateDTO.IsFree ?? existingEvent.IsFree;
            existingEvent.RequiresApproval = eventUpdateDTO.RequiresApproval ?? existingEvent.RequiresApproval;
            existingEvent.Capacity = eventUpdateDTO.Capacity ?? existingEvent.Capacity;
            existingEvent.Slug = eventUpdateDTO.Slug ?? existingEvent.Slug;
            existingEvent.ProfilePicture = eventUpdateDTO.ProfilePicture ?? existingEvent.ProfilePicture;

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
