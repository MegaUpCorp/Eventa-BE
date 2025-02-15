using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class CheckIn  : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CheckInTime { get; set; } 
    }
}
