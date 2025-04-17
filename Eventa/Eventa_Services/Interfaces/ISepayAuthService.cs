using Eventa_BusinessObject.DTOs;

namespace Eventa_Services.Interfaces;

public interface ISepayAuthService
{
    /// <summary>
    /// Gets an access token using client credentials flow
    /// </summary>
    Task<string> GetAccessTokenAsync();
    
    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    // Task<string> RefreshAccessTokenAsync(string refreshToken);
    Task<SepayTokenResponse> RefreshTokenAsync(string refreshToken);
    /// <summary>
    /// Gets the authorization URL for the authorization code flow
    /// </summary>
    string GetAuthorizationUrl(string state = null);
    
    /// <summary>
    /// Exchanges an authorization code for an access token
    /// </summary>
    Task<Eventa_BusinessObject.DTOs.SepayTokenResponse> ExchangeCodeForTokenAsync(string code);
    
    /// <summary>
    /// Revokes an access token
    /// </summary>
    Task<bool> RevokeTokenAsync(string token, string tokenTypeHint = "access_token");
}