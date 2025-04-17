using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers;

[ApiController]
[Route("api/sepay")]
public class SepayBankAccountController : ControllerBase
{
    private readonly ISepayBankAccountService _bankAccountService;
    private readonly ILogger<SepayBankAccountController> _logger;

    public SepayBankAccountController(
        ISepayBankAccountService bankAccountService,
        ILogger<SepayBankAccountController> logger)
    {
        _bankAccountService = bankAccountService;
        _logger = logger;
    }

    [HttpGet("bank-accounts")]
    public async Task<IActionResult> GetBankAccounts()
    {
        try
        {
            // Get the authorization header from the request
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return BadRequest("Authorization header is required");
            }

            string authHeaderValue = authHeader.ToString();
            
            // Extract the token from the Authorization header
            if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Bearer token is required");
            }

            string accessToken = authHeaderValue.Substring("Bearer ".Length).Trim();

            // Call the service to get bank accounts using the token
            var bankAccounts = await _bankAccountService.GetBankAccountsAsync(accessToken);
            
            return Ok(bankAccounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bank accounts");
            return StatusCode(500, "An error occurred while retrieving bank accounts");
        }
    }
}