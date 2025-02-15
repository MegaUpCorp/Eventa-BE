using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class Review
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1-5 sao
        public string Comment { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; } = false;
    }
}
