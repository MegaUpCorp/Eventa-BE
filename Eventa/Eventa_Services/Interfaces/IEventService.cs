using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEvents();
        Task<Event> GetEventById(Guid id);
        Task<string> AddEvent(CreateEventDTO eventItem, HttpContext httpContext);
        Task<bool> UpdateEvent(Guid id, UpdateEventDTO eventUpdateDTO);
        Task<bool> RemoveEvent(Guid id);
    }
}
