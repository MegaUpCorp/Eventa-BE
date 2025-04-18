using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities;

public class Transaction : BaseEntity
{


    [BsonElement("event_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid? EventId { get; set; }   // Liên kết đến sự kiện
    [BsonRepresentation(BsonType.String)]
    [BsonElement("subscriptionPlanId")]
    public Guid? SubscriptionPlanId { get; set; }

    [BsonElement("gateway")]
    [StringLength(100)]
    public string Gateway { get; set; }

    [BsonElement("transaction_date")]
    public DateTime TransactionDate { get; set; }

    [BsonElement("account_number")]
    [StringLength(100)]
    public string AccountNumber { get; set; }

    [BsonElement("sub_account")]
    [StringLength(250)]
    public string? SubAccount { get; set; } 

    [BsonElement("amount_in")]
    public decimal? AmountIn { get; set; }

    [BsonElement("amount_out")]
    public decimal AmountOut { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }    // Số tiền giao dịch (chuyển khoản)

    [BsonElement("accumulated")]
    public decimal Accumulated { get; set; }

    [BsonElement("code")]
    [StringLength(250)]
    public string? Code { get; set; } = string.Empty;

    [BsonElement("reference_number")]
    [StringLength(255)]
    public string ReferenceNumber { get; set; }

    [BsonElement("reference_code")]
    [StringLength(255)]
    public string ReferenceCode { get; set; } // Thêm mới từ payload

    [BsonElement("description")]
    public string Description { get; set; }   // Thêm mới từ payload

    [BsonElement("transaction_content")]
    public string TransactionContent { get; set; }
    [BsonElement("order_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid OrderId { get; set; }

    //[Column("body")]
    //public string Body { get; set; }
    [BsonElement("bank")]
    [StringLength(100)]
    public string Bank { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}