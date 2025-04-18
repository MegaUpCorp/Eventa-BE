using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SepayCallbackController : ControllerBase
{
    private readonly ISepayCallbackService _callbackService;
    private readonly ISepayAuthService _authService;
    private readonly ISepayService _sepayService;
    private readonly ILogger<SepayCallbackController> _logger;

    public SepayCallbackController(
        ISepayCallbackService callbackService, 
        ISepayAuthService authService,
        ISepayService sepayService,
        ILogger<SepayCallbackController> logger)
    {
        _callbackService = callbackService;
        _authService = authService;
        _sepayService = sepayService;
        _logger = logger;
    }

    [HttpPost("notify")]
    public async Task<IActionResult> HandleCallback([FromBody] SepayCallbackDto callbackData)
    {
        try
        {
            _logger.LogInformation("Received callback from SePay for order: {OrderCode}", callbackData.OrderCode);

            // Verify signature first to ensure the callback is from SePay
            bool isValidSignature = _sepayService.VerifySignature(callbackData);
            _logger.LogInformation("Signature verification result for order {OrderCode}: {IsValid}", callbackData.OrderCode, isValidSignature);

            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid signature in SePay callback for order: {OrderCode}", callbackData.OrderCode);
                return BadRequest(new { code = "97", message = "Invalid signature" });
            }

            // Process the callback after validating signature
            var result = await _callbackService.ProcessCallbackAsync(callbackData);

            // Return based on processing result
            if (result == "OK")
            {
                return Ok(new { code = "00", message = "Callback processed successfully" });
            }
            else
            {
                _logger.LogError("Error processing SePay callback: {Result}", result);
                return StatusCode(500, new { code = "99", message = "Internal error processing callback" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing SePay callback");
            return StatusCode(500, new { code = "99", message = "Internal server error" });
        }
    }

    // Return URL from SePay
    [HttpGet("return")]
    public IActionResult ReturnFromPayment([FromQuery] string orderId, [FromQuery] string status, [FromQuery] string signature)
    {
        try
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                _logger.LogWarning("Missing orderId or status in return URL");
                return Redirect("/payment-error");
            }

            _logger.LogInformation("User returned from SePay payment for order: {OrderId}, status: {Status}", orderId, status);

            // Create a basic callback DTO from query parameters for signature verification
            var callbackData = new SepayCallbackDto
            {
                OrderCode = orderId,
                Status = status,
                Signature = signature
            };

            // Verify signature
            bool isValidSignature = _sepayService.VerifySignature(callbackData);
            _logger.LogInformation("Signature verification result for return URL: {IsValid}", isValidSignature);

            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid signature in return URL for order: {OrderId}", orderId);
                return Redirect($"/payment-failed?reason=invalid-signature&orderid={orderId}");
            }

            // Redirect based on payment status
            if (status == "success" || status == "SUCCEEDED")
            {
                return Redirect($"/payment-success?orderid={orderId}");
            }
            else
            {
                return Redirect($"/payment-failed?reason={status}&orderid={orderId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay return URL");
            return Redirect("/payment-error");
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> HandleCodeCallback([FromQuery] string code)
    {
        try
        {
            _logger.LogInformation("Received authorization code from SePay: {Code}", code);

            // Exchange the authorization code for an access token
            var tokenResponse = await _authService.ExchangeCodeForTokenAsync(code);

            _logger.LogInformation("Successfully exchanged code for access token: {TokenResponse}", tokenResponse);

            // Store the tokens securely in HttpOnly cookies
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
                    Expires = DateTime.UtcNow.AddDays(30)
                });
            }

            return Ok(new
            {
                success = true,
                message = "Authentication successful",
                token = new
                {
                    access_token = tokenResponse.AccessToken,
                    token_type = tokenResponse.TokenType,
                    expires_in = tokenResponse.ExpiresIn,
                    refresh_token = tokenResponse.RefreshToken
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay authorization code");
            return StatusCode(500, new { success = false, message = "Error processing SePay authentication callback" });
        }
    }
}