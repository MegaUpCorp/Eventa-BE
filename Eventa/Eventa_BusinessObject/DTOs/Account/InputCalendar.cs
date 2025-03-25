using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class InputCalendar
    {
        [BsonElement("publicUrl")]
        public string? PublicUrl { get; set; }
    }
}
