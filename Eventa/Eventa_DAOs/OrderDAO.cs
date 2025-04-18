using Eventa_BusinessObject;
using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Eventa_DAOs
{
    public class OrderDAO : BaseDAO<Order>
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderDAO(IMongoDatabase database) : base(database, "Order")
        {
            _orders = _collection; // Initialize _orders using the BaseDAO's _collection
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new ArgumentException("Invalid Guid format for Id", nameof(id));
            }

            return await _orders.Find(o => o.Id == guidId).FirstOrDefaultAsync();
        }

        //public async Task<Order?> GetOrderByOrderCodeAsync(string orderCode)
        //{
        //    return await _orders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        //}

        public async Task<List<Order>> GetOrdersByCustomerEmailAsync(string email)
        {
            return await _orders.Find(o => o.CustomerEmail == email).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _orders.Find(o => o.Status == status).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByPaymentMethodAsync(string paymentMethod)
        {
            return await _orders.Find(o => o.PaymentMethod == paymentMethod).ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // Generate a unique order code if not provided
            //if (string.IsNullOrEmpty(order.OrderCode))
            //{
            //    order.OrderCode = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            //}

            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            // Update the updated_at timestamp
            order.UpdDate = DateTime.UtcNow;

            var result = await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string status)
        {
            var update = Builders<Order>.Update
                .Set(o => o.Status, status)
                .Set(o => o.UpdDate, DateTime.UtcNow);

            var result = await _orders.UpdateOneAsync(o => o.Id == id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new ArgumentException("Invalid Guid format for Id", nameof(id));
            }

            var result = await _orders.DeleteOneAsync(o => o.Id == guidId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        public async Task<List<Order>> GetUnpaidOrdersOlderThan(TimeSpan timeSpan)
        {
            var cutoffTime = DateTime.UtcNow - timeSpan;

            var filter = Builders<Order>.Filter.And(
                Builders<Order>.Filter.Eq(o => o.PaymentStatus, "Unpaid"),
                Builders<Order>.Filter.Lt(o => o.CreatedAt, cutoffTime)
            );

            return await _orders.Find(filter).ToListAsync();
        }

    }
}