using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Event
{
    public class CreateEventDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public int MaxParticipants { get; set; }
        [Required]
        public string TimeZone { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string Visibility { get; set; } // Public or Private
        [Required]
        public bool RequiresApproval { get; set; }
        [Required]
        public bool IsFree { get; set; }
        [Required]
        public int? Capacity { get; set; } // Null means unlimited capacity
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public string Blug { get; set; }
    }
}
