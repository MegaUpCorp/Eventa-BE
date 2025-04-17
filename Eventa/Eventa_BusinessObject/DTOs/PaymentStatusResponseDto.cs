using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class PaymentStatusResponseDto
{
    [JsonProperty("order_code")]
    public string OrderCode { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("payment_time")]
    public DateTime? PaymentTime { get; set; }
    
    [JsonProperty("payment_method")]
    public string PaymentMethod { get; set; }
    
    [JsonProperty("transaction_id")]
    public string TransactionId { get; set; }
}