//using Eventa_BusinessObject.Entities;
//using Eventa_DAOs;
//using Eventa_Services.Interfaces;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Eventa_Services.Implements
//{
//    public class EventService : IEventService
//    {
//        private readonly EventDAO _eventDAO;
//        private readonly IOrganizerService _organizerService;
//        private readonly IAccountService _accountService;

//        public EventService(IMongoDatabase database, IOrganizerService organizerService, IAccountService accountService)
//        {
//            _eventDAO = new EventDAO(database);
//            _organizerService = organizerService;
//            _accountService = accountService;
//        }

//        public async Task<List<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default)
//        {
//            return await _eventDAO.GetAllEventsAsync(null, cancellationToken);
//        }

//        public async Task<Event?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
//        {
//            return await _eventDAO.GetEventByIdAsync(id, cancellationToken);
//        }

//        public async Task<bool> AddEventAsync(Event eventToAdd, CancellationToken cancellationToken = default)
//        {
//            var organizer = await _organizerService.GetOrganizerByIdAsync(eventToAdd.OrganizerId, cancellationToken);
//            var account = await _accountService.GetAccountByIdAsync(eventToAdd.OrganizerId, cancellationToken);

//            if (organizer == null || account == null)
//            {
//                return false;
//            }

//            return await _eventDAO.AddEventAsync(eventToAdd, cancellationToken);
//        }

//        public async Task<bool> UpdateEventAsync(Event eventToUpdate, CancellationToken cancellationToken = default)
//        {
//            var organizer = await _organizerService.GetOrganizerByIdAsync(eventToUpdate.OrganizerId, cancellationToken);
//            var account = await _accountService.GetAccountByIdAsync(eventToUpdate.OrganizerId, cancellationToken);

//            if (organizer == null || account == null)
//            {
//                return false;
//            }

//            return await _eventDAO.UpdateEventAsync(eventToUpdate, cancellationToken);
//        }

//        public async Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default)
//        {
//            return await _eventDAO.DeleteEventAsync(id, cancellationToken);
//        }
//    }
//}