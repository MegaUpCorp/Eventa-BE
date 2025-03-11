using Eventa_BusinessObject.Entities;
using Eventa_BusinessObject;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventa_Services.Interfaces;

namespace Eventa_Services.Implements
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<Event> _context;

        public EventRepository(EventaDBContext context)
        {
            _context = context.Events;
        }

        public async Task AddEvent(Event eventItem)
        {
            await _context.InsertOneAsync(eventItem);
        }

        public async Task RemoveEvent(Guid id)
        {
            var update = Builders<Event>.Update.Set(x => x.DelFlg, true);
            await _context.FindOneAndUpdateAsync(e => e.Id == id, update);
        }

        public async Task<List<Event>> GetAll()
        {
            var projection = Builders<Event>.Projection
                .Include(e => e.CalendarId)
                .Include(e => e.Visibility)
                .Include(e => e.Title)
                .Include(e => e.StartDate)
                .Include(e => e.EndDate)
                .Include(e => e.IsOnline)
                .Include(e => e.Location)
                .Include(e => e.MeetUrl)
                .Include(e => e.Description)
                .Include(e => e.IsFree)
                .Include(e => e.RequiresApproval)
                .Include(e => e.Capacity)
                .Include(e => e.Slug)
                .Include(e => e.ProfilePicture)
                .Include(e => e.Price)
                .Include(e => e.CreatedAt)
                .Include(e => e.OrganizerId);

            return await _context.Find(e => true).Project<Event>(projection).ToListAsync();
        }

        public async Task<Event> GetById(Guid id)
        {
            return await _context.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateEvent(Guid id, Event eventItem)
        {
            var result = await _context.ReplaceOneAsync(e => e.Id == id, eventItem);
            return result.ModifiedCount > 0;
        }
        public async Task<Event> GetBySlug(string slug)
        {
            return await _context.Find(e => e.Slug == slug).FirstOrDefaultAsync();
        }
        public async Task<List<Guid>> GetOrganizerIdsByEventId(Guid eventId)
        {
            var eventWithOrganizers = await _context.Find(e => e.Id == eventId).FirstOrDefaultAsync();
            return eventWithOrganizers?.OrganizerId ?? new List<Guid>();
        }
    }
}
