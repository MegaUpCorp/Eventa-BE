using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eventa_BusinessObject.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        [BsonElement("planName")]
        public required string PlanName { get; set; }

        [BsonElement("monthlyPrice")]
        public double MonthlyPrice { get; set; }

        [BsonElement("isBilledAnnually")]
        public bool IsBilledAnnually { get; set; }

        [BsonElement("annualDiscountPercent")]
        public int AnnualDiscountPercent { get; set; }

        [BsonElement("buttonText")]
        public string? ButtonText { get; set; }

        [BsonElement("includesFreePlan")]
        public bool IncludesFreePlan { get; set; }

        [BsonElement("maxInvitationsPerWeek")]
        public int? MaxInvitationsPerWeek { get; set; }

        [BsonElement("taxCollectionEnabled")]
        public bool TaxCollectionEnabled { get; set; }

        [BsonElement("checkinManagement")]
        public bool CheckinManagement { get; set; }

        [BsonElement("customEventURL")]
        public bool CustomEventURL { get; set; }

        [BsonElement("collectFullName")]
        public bool CollectFullName { get; set; }

        [BsonElement("defaultAdminCount")]
        public int DefaultAdminCount { get; set; }

        [BsonElement("extraAdminPurchaseable")]
        public bool ExtraAdminPurchaseable { get; set; }

        [BsonElement("zapierAutomation")]
        public bool ZapierAutomation { get; set; }

        [BsonElement("apiAccess")]
        public bool APIAccess { get; set; }
        [BsonElement("BankAcc")]
        public BankAcc? BankAcc { get; set; }

        [BsonElement("features")]
        public List<string> Features { get; set; } = new();
    }
}
