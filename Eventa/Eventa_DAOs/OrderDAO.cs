using Eventa_BusinessObject;
using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class OrderDAO : BaseDAO<Order>
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderDAO(IMongoDatabase database) : base(database, "Order") { }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByOrderCodeAsync(string orderCode)
        {
            return await _orders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        }

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
            if (string.IsNullOrEmpty(order.OrderCode))
            {
                order.OrderCode = $"ORD-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }
            
            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            // Update the updated_at timestamp
            order.UpdatedAt = DateTime.UtcNow;
            
            var result = await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, string status)
        {
            var update = Builders<Order>.Update
                .Set(o => o.Status, status)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
                
            var result = await _orders.UpdateOneAsync(o => o.Id == id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            var result = await _orders.DeleteOneAsync(o => o.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}