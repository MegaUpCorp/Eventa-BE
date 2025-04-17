using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class RefundResponseDto
{
    [JsonProperty("refund_id")]
    public string RefundId { get; set; }
    
    [JsonProperty("order_code")]
    public string OrderCode { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("refund_time")]
    public DateTime RefundTime { get; set; }
    
    [JsonProperty("transaction_id")]
    public string TransactionId { get; set; }
}