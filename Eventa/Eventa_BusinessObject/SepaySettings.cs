namespace Eventa_BusinessObject;

public class SepaySettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TokenEndpoint { get; set; } = "https://oauth2.sepay.vn/token";
    public string AuthorizationEndpoint { get; set; } = "https://oauth2.sepay.vn/authorize";
    public string ApiBaseUrl { get; set; } = "https://api.sepay.vn";
    public int TokenExpiryBufferMinutes { get; set; } = 5;
    public string RedirectUri { get; set; }
    public string[] Scopes { get; set; } = new[] { "payment", "refund", "query" };
}