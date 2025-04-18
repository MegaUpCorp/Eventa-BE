using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class SubscriptionPlanDAO : BaseDAO<SubscriptionPlan>
    {
        public SubscriptionPlanDAO(IMongoDatabase database) : base(database, "SubscriptionPlans") { }

    }
}
