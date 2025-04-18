using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Eventa_BusinessObject.Entities;

[Table("tb_transactions")]
public class Transaction : BaseEntity
{


    [Column("event_id")]
    public Guid EventId { get; set; }   // Liên kết đến sự kiện

    [Column("gateway")]
    [StringLength(100)]
    public string Gateway { get; set; }

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }

    [Column("account_number")]
    [StringLength(100)]
    public string AccountNumber { get; set; }

    [Column("sub_account")]
    [StringLength(250)]
    public string? SubAccount { get; set; } 

    [Column("amount_in", TypeName = "decimal(18,2)")]
    public decimal? AmountIn { get; set; }

    [Column("amount_out", TypeName = "decimal(18,2)")]
    public decimal AmountOut { get; set; }

    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }    // Số tiền giao dịch (chuyển khoản)

    [Column("accumulated", TypeName = "decimal(18,2)")]
    public decimal Accumulated { get; set; }

    [Column("code")]
    [StringLength(250)]
    public string? Code { get; set; } = string.Empty;

    [Column("reference_number")]
    [StringLength(255)]
    public string ReferenceNumber { get; set; }

    [Column("reference_code")]
    [StringLength(255)]
    public string ReferenceCode { get; set; } // Thêm mới từ payload

    [Column("description")]
    public string Description { get; set; }   // Thêm mới từ payload

    [Column("transaction_content")]
    public string TransactionContent { get; set; }

    //[Column("body")]
    //public string Body { get; set; }
    [Column("bank")]
    [StringLength(100)]
    public string Bank { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}