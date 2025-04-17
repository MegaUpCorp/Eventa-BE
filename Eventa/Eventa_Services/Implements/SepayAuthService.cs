using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Eventa_Services.Implements;

public class SepayAuthService : ISepayAuthService
{
    private readonly HttpClient _httpClient;
    private readonly SepaySettings _settings;
    private SepayTokenResponse _tokenResponse;
    private readonly object _lockObject = new object();

    public SepayAuthService(IOptions<SepaySettings> settings)
    {
        _settings = settings.Value;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        // Check if we have a valid token already
        if (_tokenResponse != null && !_tokenResponse.IsExpired(_settings.TokenExpiryBufferMinutes))
        {
            return _tokenResponse.AccessToken;
        }

        // Lock to prevent multiple threads from requesting tokens simultaneously
        lock (_lockObject)
        {
            // Double-check in case another thread already fetched a new token
            if (_tokenResponse != null && !_tokenResponse.IsExpired(_settings.TokenExpiryBufferMinutes))
            {
                return _tokenResponse.AccessToken;
            }
        }

        // Get a new token
        return await RequestNewAccessTokenAsync();
    }

    private async Task<string> RequestNewAccessTokenAsync()
    {
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _settings.ClientId },
            { "client_secret", _settings.ClientSecret },
            { "scope", string.Join(" ", _settings.Scopes) }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        // Set accept header to application/json as required by SePay
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to obtain SePay access token. Status: {response.StatusCode}, Error: {errorContent}");
        }

        var json = await response.Content.ReadAsStringAsync();
        _tokenResponse = JsonConvert.DeserializeObject<SepayTokenResponse>(json);
        
        if (_tokenResponse == null)
        {
            throw new Exception("Failed to deserialize SePay token response");
        }

        return _tokenResponse.AccessToken;
    }

    public async Task<string> RefreshAccessTokenAsync(string refreshToken)
    {
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", _settings.ClientId },
            { "client_secret", _settings.ClientSecret }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        // Set accept header to application/json as required by SePay
        // request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to refresh SePay access token. Status: {response.StatusCode}, Error: {errorContent}");
        }

        var json = await response.Content.ReadAsStringAsync();
        _tokenResponse = JsonConvert.DeserializeObject<SepayTokenResponse>(json);
        
        if (_tokenResponse == null)
        {
            throw new Exception("Failed to deserialize SePay token response");
        }

        return _tokenResponse.AccessToken;
    }

    public async Task<SepayTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // According to SePay documentation in the screenshot
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret }
            };

            // Create request content with form URL encoded content type
            var content = new FormUrlEncodedContent(requestBody);
            
            // Set up the request with proper headers as specified in SePay docs
            var request = new HttpRequestMessage(HttpMethod.Post, "https://my.sepay.vn/oauth/token")
            {
                Content = content
            };
            
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            // request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send the request to SePay
            var response = await _httpClient.SendAsync(request);
            
            // Process the response
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to refresh token. Status: {response.StatusCode}, Error: {errorContent}");
            }

            // Parse the token response
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SepayTokenResponse>(json);
            
            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize SePay token response");
            }
            
            // Cache the token response
            _tokenResponse = tokenResponse;
            
            return tokenResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error refreshing token: {ex.Message}", ex);
        }
    }
    
    public string GetAuthorizationUrl(string state = null)
    {
        // Build the authorization URL according to OAuth 2.0 and SePay specs
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", _settings.ClientId },
            { "redirect_uri", _settings.RedirectUri },
            { "scope", string.Join(" ", _settings.Scopes) }
        };
        
        // Add state parameter if provided (recommended for CSRF protection)
        if (!string.IsNullOrEmpty(state))
        {
            queryParams.Add("state", state);
        }
        
        // Construct the authorization URL with query parameters
        var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
        return $"{_settings.AuthorizationEndpoint}?{queryString}";
    }
    
    public async Task<SepayTokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        try
        {
            // Build the token request according to SePay documentation
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _settings.RedirectUri },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret }
            };

            // Create request content with form URL encoded content type
            var content = new FormUrlEncodedContent(requestBody);
            
            // Set up the request with proper headers as specified in SePay docs
            var request = new HttpRequestMessage(HttpMethod.Post, "https://my.sepay.vn/oauth/token")
            {
                Content = content
            };
            // request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // Send the request to SePay
            var response = await _httpClient.SendAsync(request);
            
            // Process the response
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to exchange authorization code for token. Status: {response.StatusCode}, Error: {errorContent}");
            }

            // Parse the token response
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SepayTokenResponse>(json);
            
            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize SePay token response");
            }
            
            // Cache the token response
            _tokenResponse = tokenResponse;
            
            return tokenResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error exchanging code for token: {ex.Message}", ex);
        }
    }
    
    public async Task<bool> RevokeTokenAsync(string token, string tokenTypeHint = "access_token")
    {
        // According to OAuth 2.0 Token Revocation (RFC 7009)
        var requestBody = new Dictionary<string, string>
        {
            { "token", token },
            { "token_type_hint", tokenTypeHint },
            { "client_id", _settings.ClientId },
            { "client_secret", _settings.ClientSecret }
        };

        var content = new FormUrlEncodedContent(requestBody);
        
        // SePay documentation should specify the revocation endpoint
        // Assuming it follows standard OAuth 2.0 patterns
        var revocationEndpoint = $"{_settings.TokenEndpoint}/revoke";
        
        var response = await _httpClient.PostAsync(revocationEndpoint, content);
        
        // Token revocation should return 200 OK if successful
        return response.IsSuccessStatusCode;
    }
}
