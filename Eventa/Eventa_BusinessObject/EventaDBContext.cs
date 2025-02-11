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
    }
}
