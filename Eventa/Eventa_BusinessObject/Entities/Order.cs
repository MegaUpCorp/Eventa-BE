using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities;

[Table("tb_orders")]
public class Order : BaseEntity
{
    [BsonId]
    [Key]
    [BsonRepresentation(BsonType.ObjectId)]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("total")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Total { get; set; } = 0;
    
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; }
    
    [Column("order_code")]
    [StringLength(100)]
    public string OrderCode { get; set; }
    
    [Column("payment_method")]
    [StringLength(50)]
    public string PaymentMethod { get; set; }
    
    [Column("customer_name")]
    [StringLength(255)]
    public string CustomerName { get; set; }
    
    [Column("customer_email")]
    [StringLength(255)]
    public string CustomerEmail { get; set; }
    
    [Column("customer_phone")]
    [StringLength(20)]
    public string CustomerPhone { get; set; }
    
    [Column("transaction_id")]
    [StringLength(255)]
    public string TransactionId { get; set; }
    
    // Foreign key reference to Transaction entity
    [ForeignKey("TransactionId")]
    public Transaction Transaction { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    
    [Column("note")]
    public string Note { get; set; }
}