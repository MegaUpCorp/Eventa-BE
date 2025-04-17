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
    private readonly ILogger<SepayCallbackController> _logger;

    public SepayCallbackController(
        ISepayCallbackService callbackService, 
        ISepayAuthService authService,
        ILogger<SepayCallbackController> logger)
    {
        _callbackService = callbackService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("notify")]
    public async Task<IActionResult> HandleCallback([FromBody] SepayCallbackDto callbackData)
    {
        try
        {
            _logger.LogInformation("Received callback from SePay for order: {OrderCode}", callbackData.OrderCode);
            
            // Process the callback
            var result = await _callbackService.ProcessCallbackAsync(callbackData);
            
            // Return based on processing result
            if (result == "OK")
            {
                return Ok(new { success = true, message = "Callback processed successfully" });
            }
            else if (result == "INVALID_SIGNATURE")
            {
                _logger.LogWarning("Invalid signature in SePay callback for order: {OrderCode}", callbackData.OrderCode);
                return BadRequest(new { success = false, message = "Invalid signature" });
            }
            else
            {
                _logger.LogError("Error processing SePay callback: {Result}", result);
                return StatusCode(500, new { success = false, message = "Internal error processing callback" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing SePay callback");
            return StatusCode(500, new { success = false, message = "Internal server error" });
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
            
            _logger.LogInformation("Successfully exchanged code for access token");
            
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
            
            // Redirect to a success page
            return RedirectToAction("PaymentProcess", "Payment");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay authorization code");
            return StatusCode(500, "Error processing SePay authentication callback");
        }
    }
}