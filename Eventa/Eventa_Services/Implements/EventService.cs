using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
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
        private readonly ICategoryRepository _categoryRepository;

        public EventService(IEventRepository eventRepository, IOrganizerRepository organizerRepository, ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _organizerRepository = organizerRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Event>> GetAllEvents()
        {
            return await _eventRepository.GetAll();
        }

        public async Task<Event> GetEventById(Guid id)
        {
            return await _eventRepository.GetById(id);
        }

        public async Task CreateEvent(CreateEventDTO eventItem, HttpContext httpContext)
        {
            var accountID = UserUtil.GetAccountId(httpContext);
            if (accountID == null)
            {
                var response = new
                {
                    message = "Account not found"
                };
            }


            var category = await _categoryRepository.GetCategoryById(eventItem.CategoryId);
            if (category == null)
            {
                throw new Exception("Category does not exist.");
            }
            var Events  = new Event
            {
                Name = eventItem.Name,
                Description = eventItem.Description,
                Category = category,
                Organizer = await _organizerRepository.GetOrganizerById(accountID.Value),
                StartDate = eventItem.StartDate,
                EndDate = eventItem.EndDate,
                Location = eventItem.Location,
                Price = eventItem.Price,
                Capacity = eventItem.Capacity,
                ImageUrl = eventItem.ImageUrl
             };


                  await _eventRepository.AddEvent(Events);
        }

        

        public async Task<bool> UpdateEvent(Guid id, Event eventItem)
        {
            return await _eventRepository.UpdateEvent(id, eventItem);
        }

        public async Task<bool> DeleteEvent(Guid id)
        {
            await _eventRepository.RemoveEvent(id);
            return true;

        }
    }
}
