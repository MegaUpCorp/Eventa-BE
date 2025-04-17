using Newtonsoft.Json;

namespace Eventa_BusinessObject.DTOs;

public class RefreshTokenRequest
{
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
}