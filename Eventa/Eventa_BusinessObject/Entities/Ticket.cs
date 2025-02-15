using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Ticket
    {
        public Guid EventId { get; set; }
        public string TicketType { get; set; } = "Standard"; // "Standard", "VIP", "Free"
        public decimal Price { get; set; } = 0;
        public int QuantityAvailable { get; set; }
        public bool IsSoldOut { get; set; } = false;
        public bool IsRefundable { get; set; } = true;
        public string Currency { get; set; } = "USD";
    }
}
