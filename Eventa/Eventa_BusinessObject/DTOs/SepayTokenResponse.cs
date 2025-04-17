using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class SepayTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsExpired(int bufferMinutes = 5)
    {
        return DateTime.UtcNow >= IssuedAt.AddSeconds(ExpiresIn).AddMinutes(-bufferMinutes);
    }
}