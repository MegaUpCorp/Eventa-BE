using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public string CoverImage { get; set; } = string.Empty;
        public Guid OrganizerId { get; set; }  
        public bool IsPublic { get; set; } = true; 
        public Guid CategoryId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
