using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Eventa_Services.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class EventService : IEventService
    {

        private readonly IEventRepository _eventRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IAccountRepository _accountRepository;
        public EventService(IEventRepository eventRepository, IOrganizerRepository organizerRepository, IAccountRepository accountRepository)
        {
            _eventRepository = eventRepository;
            _organizerRepository = organizerRepository;
            _accountRepository = accountRepository;
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
                    Id = eventItem.Location.Id,
                    Name = eventItem.Location.Name,
                    Address = eventItem.Location.Address,
                    Latitude = (double)eventItem.Location.Lat,
                    Longitude = (double)eventItem.Location.Lng
                },
                MeetUrl = eventItem.IsOnline ? eventItem.MeetUrl : null,
                Description = eventItem.Description,
                IsFree = eventItem.IsFree ?? true, // Default to true if null
                RequiresApproval = eventItem.RequiresApproval,
                Capacity = eventItem.Capacity,
                Slug = eventItem.Slug,
                ProfilePicture = string.IsNullOrWhiteSpace(eventItem.ProfilePicture) ? null : eventItem.ProfilePicture,
                CreatedAt = DateTime.UtcNow,
                OrganizerId = organizer.Id
            };
            var newCarlandar = new Eventa_BusinessObject.Entities.Calendar
            {
                Name = newEvent.CalendarId,
                AccountId = organizer.Id
            };

            await _eventRepository.AddEvent(newEvent);
            var result = await _accountRepository.AddCalendarAsync(newCarlandar);
            return "Event created successfully.";
        }


        public async Task<bool> UpdateEvent(Guid id, UpdateEventDTO eventUpdateDTO)
        {
            var existingEvent = await _eventRepository.GetById(id);
            if (existingEvent == null)
            {
                return false;
            }

            existingEvent.CalendarId ??= eventUpdateDTO.CalendarId;
            existingEvent.Visibility ??= eventUpdateDTO.Visibility;
            existingEvent.Title ??= eventUpdateDTO.Title;
            existingEvent.Description ??= eventUpdateDTO.Description;
            existingEvent.StartDate = eventUpdateDTO.StartDate ?? existingEvent.StartDate;
            existingEvent.EndDate = eventUpdateDTO.EndDate ?? existingEvent.EndDate;
            existingEvent.IsOnline = eventUpdateDTO.IsOnline ?? existingEvent.IsOnline;

            if (eventUpdateDTO.IsOnline == true)
            {
                existingEvent.MeetUrl = eventUpdateDTO.MeetUrl ?? existingEvent.MeetUrl;
                existingEvent.Location = null; // Xóa location khi là event online
            }
            else if (eventUpdateDTO.IsOnline == false && eventUpdateDTO.Location != null)
            {
                existingEvent.Location ??= new Location(); // Đảm bảo không bị null

                existingEvent.Location.Id = eventUpdateDTO.Location.Id ?? existingEvent.Location.Id;
                existingEvent.Location.Name = eventUpdateDTO.Location.Name ?? existingEvent.Location.Name;
                existingEvent.Location.Address = eventUpdateDTO.Location.Address ?? existingEvent.Location.Address;

                if (eventUpdateDTO.Location.Lat.HasValue)
                    existingEvent.Location.Latitude = eventUpdateDTO.Location.Lat.Value;

                if (eventUpdateDTO.Location.Lng.HasValue)
                    existingEvent.Location.Longitude = eventUpdateDTO.Location.Lng.Value;
            }

            existingEvent.IsFree = eventUpdateDTO.IsFree ?? existingEvent.IsFree;
            existingEvent.RequiresApproval = eventUpdateDTO.RequiresApproval ?? existingEvent.RequiresApproval;
            existingEvent.Capacity = eventUpdateDTO.Capacity ?? existingEvent.Capacity;
            existingEvent.Slug ??= eventUpdateDTO.Slug;
            existingEvent.ProfilePicture ??= eventUpdateDTO.ProfilePicture;

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
