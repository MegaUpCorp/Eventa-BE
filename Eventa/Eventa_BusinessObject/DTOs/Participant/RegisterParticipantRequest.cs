using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Participant
{
    public class RegisterParticipantRequest
    {
        public Guid EventId { get; set; }
    }
}
