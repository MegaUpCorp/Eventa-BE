using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Organizer
{
     public class OrganizerDTO
    {
        [BsonElement("organizerDescription")]
        public string? OrganizerDescription { get; set; }
    }
}
