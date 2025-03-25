using System;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.DTOs.Event
{
    public class UpdateEventDTO
    {
        public Guid? CalendarId { get; set; }
        public string? Visibility { get; set; }
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsOnline { get; set; }
        public LocationDTOs? Location { get; set; }
        public string? MeetUrl { get; set; }
        public string? Description { get; set; }
        public bool? IsFree { get; set; }
        public bool? RequiresApproval { get; set; }
        public int? Capacity { get; set; }
        public string? Slug { get; set; }
        public string? ProfilePicture { get; set; }
        public float? Price { get; set; }
    }
}
