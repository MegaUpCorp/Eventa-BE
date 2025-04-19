using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.CheckIn
{
    public class CheckInRequest
    {
        public Guid ParticipantId { get; set; }
        public Guid EventId { get; set; }
        public string UniqueCode { get; set; }
    }
}
