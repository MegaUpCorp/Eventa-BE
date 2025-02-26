using Eventa_BusinessObject.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject
{
    public class EventaDBContext
    {
        private readonly IMongoDatabase _database;

        public EventaDBContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<Account> Accounts => _database.GetCollection<Account>("Accounts");
        public IMongoCollection<Organizer> Organizers => _database.GetCollection<Organizer>("Organizers");
        public IMongoCollection<Event> Events => _database.GetCollection<Event>("Event");
        public IMongoCollection<Participant> Participants => _database.GetCollection<Participant>("Participants");
        public IMongoCollection<CheckIn> CheckIns => _database.GetCollection<CheckIn>("CheckIns");
        public IMongoCollection<Ticket> SessionParticipants => _database.GetCollection<Ticket>("Tickets");
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
    }
}
