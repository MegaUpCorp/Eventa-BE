using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class SepayCallbackDto
{
    [JsonProperty("order_code")]
    public string OrderCode { get; set; }
    
    [JsonProperty("payment_id")]
    public string PaymentId { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("transaction_time")]
    public DateTime TransactionTime { get; set; }
    
    [JsonProperty("signature")]
    public string Signature { get; set; }
    
    [JsonProperty("merchant_id")]
    public string MerchantId { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("transaction_id")]
    public string TransactionId { get; set; }
    
    [JsonProperty("payment_method")]
    public string PaymentMethod { get; set; }
}