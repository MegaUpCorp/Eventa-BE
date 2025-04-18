using Eventa_BusinessObject;
using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class TransactionDAO : BaseDAO<Transaction>
    {
        public TransactionDAO(IMongoDatabase database) : base(database, "Transactions")
        {
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid id)
        {
            return await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Transaction> GetTransactionByReferenceNumberAsync(string referenceNumber)
        {
            return await _collection.Find(t => t.ReferenceNumber == referenceNumber).FirstOrDefaultAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByGatewayAsync(string gateway)
        {
            return await _collection.Find(t => t.Gateway == gateway).ToListAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _collection.InsertOneAsync(transaction);
            return transaction;
        }

        public async Task<bool> UpdateTransactionAsync(Transaction transaction)
        {
            var result = await _collection.ReplaceOneAsync(t => t.Id == transaction.Id, transaction);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteTransactionAsync(Guid id)
        {
            var result = await _collection.DeleteOneAsync(t => t.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}