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

        [Required]
        public bool IsFree { get; set; } = true;

        [Required]
        public bool RequiresApproval { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public string ProfilePicture { get; set; }
    }
}
