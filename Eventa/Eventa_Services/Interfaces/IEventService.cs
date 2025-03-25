using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        Task<List<Event>> GetEventsByAccountId(HttpContext httpContext);
        Task<List<object>> GetEventsByFilter(string? publicUrl, string? title, DateTime? startDate,HttpContext httpContext);
        Task<bool> CheckUserAccessToEvent(string slug, HttpContext httpContext);
        Task<List<AccountDTO>> GetSubCribedCalendar(string slug);
        Task<bool> UpdateEventBySlug(string slug, UpdateEventDTO eventUpdateDTO);
        Task<Event> GetEventBySlug(string slug);
            }

}
