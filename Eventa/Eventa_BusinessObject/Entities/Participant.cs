using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public  class Participant
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsCheckedIn { get; set; } = false;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? CheckedInAt { get; set; }
        public Guid TicketId { get; set; }
    }
}
