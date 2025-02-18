using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Ticket : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid EventId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ParticipantId { get; set; }

        public string TicketType { get; set; } // VIP, Standard, etc.

        public decimal Price { get; set; } // Giá vé

        public bool IsUsed { get; set; } // Đánh dấu đã sử dụng hay chưa

        public DateTime IssuedAt { get; set; } // Thời gian phát hành vé
    }
}
