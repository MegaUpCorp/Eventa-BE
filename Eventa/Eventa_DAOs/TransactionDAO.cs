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
        private readonly IMongoCollection<Transaction> _transactions;

        public TransactionDAO(IMongoDatabase database) : base(database, "Transaction") { }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _transactions.Find(_ => true).ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(string id)
        {
            return await _transactions.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Transaction> GetTransactionByReferenceNumberAsync(string referenceNumber)
        {
            return await _transactions.Find(t => t.ReferenceNumber == referenceNumber).FirstOrDefaultAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByGatewayAsync(string gateway)
        {
            return await _transactions.Find(t => t.Gateway == gateway).ToListAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _transactions.InsertOneAsync(transaction);
            return transaction;
        }

        public async Task<bool> UpdateTransactionAsync(Transaction transaction)
        {
            var result = await _transactions.ReplaceOneAsync(t => t.Id == transaction.Id, transaction);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteTransactionAsync(string id)
        {
            var result = await _transactions.DeleteOneAsync(t => t.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}