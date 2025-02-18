using System;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.DTOs.Event
{
    public class UpdateEventDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? MaxParticipants { get; set; }
        public string? TimeZone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Visibility { get; set; } // Public or Private  
        public bool? RequiresApproval { get; set; }
        public bool? IsFree { get; set; }
        public int? Capacity { get; set; } // Null means unlimited capacity  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Blug { get; set; }
    }
}
