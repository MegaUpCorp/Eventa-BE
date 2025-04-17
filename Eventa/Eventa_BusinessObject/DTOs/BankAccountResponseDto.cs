using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class BankAccountResponseDto
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("label")]
    public string Label { get; set; }
    
    [JsonProperty("account_holder_name")]
    public string AccountHolderName { get; set; }
    
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("accumulated")]
    public decimal Accumulated { get; set; }
    
    [JsonProperty("active")]
    public bool Active { get; set; }
    
    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
    
    [JsonProperty("bank")]
    public BankInfoDto Bank { get; set; }
}

public class BankInfoDto
{
    [JsonProperty("short_name")]
    public string ShortName { get; set; }
    
    [JsonProperty("full_name")]
    public string FullName { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("bin")]
    public string Bin { get; set; }
    
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }
    
    [JsonProperty("logo_url")]
    public string LogoUrl { get; set; }
}

public class BankAccountListResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("data")]
    public List<BankAccountResponseDto> Data { get; set; }
}