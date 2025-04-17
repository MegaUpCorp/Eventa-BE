using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class RefundRequestDto
{
    [JsonProperty("order_code")]
    public string OrderCode { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("reason")]
    public string Reason { get; set; }
    
    [JsonProperty("refund_id")]
    public string RefundId { get; set; }
}