using Eventa_BusinessObject.DTOs;
using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Eventa_Services.Implements.SepayPaymentService;

namespace Eventa_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SepayAuthController : ControllerBase
{
    private readonly ISepayAuthService _sepayAuthService;
    private readonly ILogger<SepayAuthController> _logger;
    private readonly ISepayService _sepayService;

    public SepayAuthController(ISepayAuthService sepayAuthService, ILogger<SepayAuthController> logger, ISepayService sepayService)
    {
        _sepayAuthService = sepayAuthService;
        _logger = logger;
        _sepayService = sepayService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        try
        {
            // Generate a random state to protect against CSRF
            var state = Guid.NewGuid().ToString();
            
            // Store the state in session for verification when callback happens
            if (HttpContext.Session != null)
            {
                HttpContext.Session.SetString("SepayAuthState", state);
            }
            else
            {
                _logger.LogWarning("Session is not available. CSRF protection will be limited.");
                // Fallback to using a temporary state without session
                state = "no-session-" + state;
            }
            
            // Get the authorization URL from the SepayAuthService
            var authorizationUrl = _sepayAuthService.GetAuthorizationUrl(state);
            
            _logger.LogInformation("Redirecting to SePay authorization URL: {Url}", authorizationUrl);
            
            // Redirect the user to SePay authorization page
            return Redirect(authorizationUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating SePay authorization flow");
            return StatusCode(500, new { error = "server_error", error_description = "Error initiating SePay login process" });
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state = null)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("No authorization code provided in callback");
                return BadRequest(new { error = "invalid_request", error_description = "Authorization code is required" });
            }
            
            _logger.LogInformation("Received authorization code from SePay");
            
            // State validation (optional if SePay doesn't always return it)
            if (!string.IsNullOrEmpty(state) && HttpContext.Session != null)
            {
                var savedState = HttpContext.Session.GetString("SepayAuthState");
                if (string.IsNullOrEmpty(savedState) || savedState != state)
                {
                    _logger.LogWarning("State mismatch in callback. Expected: {Expected}, Received: {Received}", 
                        savedState, state);
                    // We'll continue anyway since we have a valid code
                }
            }
            
            // Exchange the authorization code for an access token
            var tokenResponse = await _sepayAuthService.ExchangeCodeForTokenAsync(code);
            
            _logger.LogInformation("Successfully exchanged code for access token. Token expires in {ExpiresIn} seconds", 
                tokenResponse.ExpiresIn);
            
            // Store the tokens securely in cookies
            Response.Cookies.Append("SepayAccessToken", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            });
            
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                Response.Cookies.Append("SepayRefreshToken", tokenResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(30) // Refresh tokens typically last longer
                });
            }
            
            // Save token info in session if available
            if (HttpContext.Session != null)
            {
                HttpContext.Session.SetString("SepayTokenType", tokenResponse.TokenType);
                HttpContext.Session.SetString("SepayTokenExpiresAt", 
                    DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o"));
            }
            
            // Return the token response directly in the API response
            return Ok(new 
            { 
                success = true, 
                message = "Authentication successful",
                token = new
                {
                    access_token = tokenResponse.AccessToken,
                    token_type = tokenResponse.TokenType,
                    expires_in = tokenResponse.ExpiresIn,
                    refresh_token = tokenResponse.RefreshToken,
                    issued_at = tokenResponse.IssuedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay callback: {Message}", ex.Message);
            return StatusCode(500, new { error = "server_error", error_description = "Error processing SePay authentication callback" });
        }
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Get the access token from cookies
            if (Request.Cookies.TryGetValue("SepayAccessToken", out var accessToken))
            {
                try
                {
                    // Revoke the token at SePay
                    await _sepayAuthService.RevokeTokenAsync(accessToken);
                }
                catch (Exception ex)
                {
                    // Log error but continue to clean up tokens locally
                    _logger.LogWarning(ex, "Error revoking token at SePay, but proceeding with local cleanup");
                }
                
                // Remove the cookies regardless of token revocation result
                Response.Cookies.Delete("SepayAccessToken");
                Response.Cookies.Delete("SepayRefreshToken");
            }
            
            // Clean up session if available
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Remove("SepayTokenType");
                HttpContext.Session.Remove("SepayTokenExpiresAt");
                HttpContext.Session.Remove("SepayAuthState");
            }
            
            return Ok(new { success = true, message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SePay logout");
            return StatusCode(500, new { error = "server_error", error_description = "Error during logout process" });
        }
    }

    /// <summary>
    /// Refreshes an access token using a refresh token according to SePay documentation
    /// </summary>
    /// <returns>New token response with updated access token and refresh token</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            _logger.LogInformation("Refreshing access token with refresh token");
            
            // Check if refresh token was provided in the request
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                // Try to get refresh token from cookie as fallback
                if (Request.Cookies.TryGetValue("SepayRefreshToken", out var cookieRefreshToken))
                {
                    _logger.LogInformation("Using refresh token from cookie");
                    request = new RefreshTokenRequest { RefreshToken = cookieRefreshToken };
                }
                else
                {
                    return BadRequest(new { error = "invalid_request", error_description = "Refresh token is required" });
                }
            }
            
            // Call the service method to refresh the token
            var tokenResponse = await _sepayAuthService.RefreshTokenAsync(request.RefreshToken);
            
            _logger.LogInformation("Successfully refreshed access token. New token expires in {ExpiresIn} seconds", 
                tokenResponse.ExpiresIn);
            
            // Update the stored tokens in cookies
            Response.Cookies.Append("SepayAccessToken", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            });
            
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                Response.Cookies.Append("SepayRefreshToken", tokenResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(30) // Refresh tokens typically last longer
                });
            }
            
            // Update the session values if available
            if (HttpContext.Session != null)
            {
                HttpContext.Session.SetString("SepayTokenType", tokenResponse.TokenType);
                HttpContext.Session.SetString("SepayTokenExpiresAt", 
                    DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o"));
            }
            
            // Return the new token information
            return Ok(new 
            { 
                success = true, 
                message = "Token refreshed successfully",
                token = new
                {
                    access_token = tokenResponse.AccessToken,
                    token_type = tokenResponse.TokenType,
                    expires_in = tokenResponse.ExpiresIn,
                    refresh_token = tokenResponse.RefreshToken,
                    issued_at = tokenResponse.IssuedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token: {Message}", ex.Message);
            return StatusCode(500, new { error = "server_error", error_description = "Error refreshing token" });
        }


    }
    [HttpPost("generate-qr")]
    public async Task<IActionResult> GenerateQr([FromBody] EventDTO dto)
    {
        try
        {
            var (qrUrl, createdOrder) = await _sepayService.GenerateSePayQrUrlAsync(dto);

            return Ok(new
            {
                qrUrl,
                order = new
                {
                    createdOrder.Id,
                    createdOrder.EventId,
                    createdOrder.Total,
                    createdOrder.PaymentStatus,
                    createdOrder.OrderType,
                    createdOrder.PaymentMethod,
                    createdOrder.Name,
                    createdOrder.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] SepayWebhookPayload payload)
    {
        await _sepayService.HandleWebhookAsync(payload,HttpContext);

        // Trả về JSON có success: true và HTTP 200
        return Ok(new { success = true });
    }
    [HttpPost("create-transaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] SepayWebhookPayload  transaction)
    {
        try
        {
            await _sepayService.CreateTransaction(transaction);
            return Ok(new { success = true, message = "Transaction created successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, new { error = "server_error", error_description = "Error creating transaction" });
        }
    }
    [HttpGet("get-all-transactions")]
    public async Task<IActionResult> GetAllTransactions()
    {
        try
        {
            var transactions = await _sepayService.GetAllTransactions();
            return Ok(new { success = true, transactions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, new { error = "server_error", error_description = "Error retrieving transactions" });
        }
    }
    [HttpPost("cancel-expired-orders")]
    public async Task<IActionResult> CancelExpiredOrders()
    {
        try
        {
            await _sepayService.CancelExpiredOrdersAsync();
            return Ok(new { success = true, message = "Expired orders canceled successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling expired orders");
            return StatusCode(500, new { error = "server_error", error_description = "Error canceling expired orders" });
        }
    }
    [HttpPost("orders/{id}/refund")]
    public async Task<IActionResult> RefundOrder(Guid id, [FromBody] string reason)
    {
        await _sepayService.RefundOrderAsync(id, reason);
        return Ok("Đã xử lý hoàn tiền.");
    }
    [HttpPost("createSubscriptionPlan")]
    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionPlan plan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPlan = await _sepayService.CreateSubscriptionPlan(plan);
        return Ok(new { success = true, message = " SubscriptionPlan created successfully." });
    }
    [HttpGet("paymentsCheckingStatus")]
    public async Task<IActionResult> PaymentsCheckingStatus(Guid orderId)
    {
        try
        {
            var status = await _sepayService.CheckStatusOrder(orderId);
            return Ok(new { status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status");
            return StatusCode(500, new { error = "server_error", error_description = "Error checking payment status" });
        }
    }

}