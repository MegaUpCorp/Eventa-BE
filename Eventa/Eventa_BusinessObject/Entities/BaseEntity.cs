using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.Entities
{
    public class BaseEntity
    {
        public BaseEntity() {
            InsDate = DateTime.Now;
            UpdDate = DateTime.Now;
            DelFlg = false;
        }
        public virtual Guid Id { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public bool DelFlg { get; set; } = false;
    }
}
