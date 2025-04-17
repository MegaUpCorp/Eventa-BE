using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class PaymentRequestDto
{
    [JsonProperty("amount")]
    public string amount { get; set; }
    
    [JsonProperty("currency")]
    public string currency { get; set; } = "VND";
    
    [JsonProperty("order_id")]
    public string order_id { get; set; }
    
    [JsonProperty("order_info")]
    public string order_info { get; set; }
    
    [JsonProperty("return_url")]
    public string return_url { get; set; }
    
    [JsonProperty("notify_url")]
    public string notify_url { get; set; }
    
    [JsonProperty("payment_methods")]
    public string[] payment_methods { get; set; }
    
    [JsonProperty("customer_email")]
    public string customer_email { get; set; }
    
    [JsonProperty("customer_phone")]
    public string customer_phone { get; set; }
    
    [JsonProperty("customer_name")]
    public string customer_name { get; set; }
    
    [JsonProperty("language")]
    public string language { get; set; } = "vn";
    
    [JsonProperty("signature")]
    public string signature { get; set; }
}