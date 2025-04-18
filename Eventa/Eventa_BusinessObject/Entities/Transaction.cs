using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities;

[Table("tb_transactions")]
public class Transaction : BaseEntity
{

    
    [Column("gateway")]
    [StringLength(100)]
    public string Gateway { get; set; }
    
    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [Column("account_number")]
    [StringLength(100)]
    public string AccountNumber { get; set; }
    
    [Column("sub_account")]
    [StringLength(250)]
    public string SubAccount { get; set; }
    
    [Column("amount_in")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal AmountIn { get; set; } = 0;
    
    [Column("amount_out")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal AmountOut { get; set; } = 0;
    
    [Column("accumulated")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Accumulated { get; set; } = 0;
    
    [Column("code")]
    [StringLength(250)]
    public string Code { get; set; }
    
    [Column("transaction_content")]
    public string TransactionContent { get; set; }
    
    [Column("reference_number")]
    [StringLength(255)]
    public string ReferenceNumber { get; set; }
    
    [Column("body")]
    public string Body { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}