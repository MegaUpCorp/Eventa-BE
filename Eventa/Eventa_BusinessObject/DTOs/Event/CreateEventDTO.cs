using System;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.DTOs.Event
{
    public class LocationDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class CreateEventDTO
    {
        [Required]
        public string CalendarId { get; set; }

        [Required]
        public string Visibility { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        public LocationDTO Location { get; set; }

        public string MeetUrl { get; set; }

        public string Description { get; set; }

        public bool? IsFree { get; set; }

        [Required]
        public bool RequiresApproval { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string Slug { get; set; }

        // Made ProfilePicture nullable to account for empty string
        public string? ProfilePicture { get; set; }
    }
}
