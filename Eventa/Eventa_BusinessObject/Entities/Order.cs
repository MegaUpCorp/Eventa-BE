using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities;

public class Order : BaseEntity
{
    [BsonRepresentation(BsonType.String)]
    [BsonElement("eventID")]
    public Guid? EventId { get; set; }
    [BsonElement("total")]
    public double Total { get; set; } = 0;
    [BsonRepresentation(BsonType.String)]
    [BsonElement("subscriptionPlanId")]
    public Guid? SubscriptionPlanId{ get; set; }
    [BsonElement("orderType")]
    [StringLength(50)]
    public string OrderType { get; set; } = "Event";

    [BsonElement("payment_status")]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid"; // Enum values: Unpaid, Paid, Cancelled, Refunded

    [BsonElement("name")]
    [StringLength(250)]
    public string Name { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("status")]
    [StringLength(50)]
    public string Status { get; set; }

    [BsonElement("payment_method")]
    [StringLength(50)]
    public string PaymentMethod { get; set; }

    [BsonElement("customer_email")]
    [StringLength(255)]
    public string CustomerEmail { get; set; }

    [BsonElement("customer_phone")]
    [StringLength(20)]
    public string CustomerPhone { get; set; }

    [BsonElement("note")]
    public string Note { get; set; }
    public DateTime? RefundDate { get; set; }
    [StringLength(255)]
    public string RefundReason { get; set; }
    public bool IsManualRefund { get; set; } = true;

}