using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Ticket
{
    public class IssueTicketDTO
    {
        public Guid EventId { get; set; }      
        public Guid ParticipantId { get; set; }
        public string TicketType { get; set; }  
        public decimal Price { get; set; }
    }
}
