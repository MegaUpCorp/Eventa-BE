using System.Net.Http.Headers;
using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Eventa_Services.Implements;

public class SepayBankAccountService : ISepayBankAccountService
{
    private readonly HttpClient _httpClient;
    private readonly SepaySettings _settings;
    private readonly ILogger<SepayBankAccountService> _logger;

    public SepayBankAccountService(
        IOptions<SepaySettings> settings,
        ILogger<SepayBankAccountService> logger)
    {
        _settings = settings.Value;
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<BankAccountListResponseDto> GetBankAccountsAsync(string accessToken)
    {
        try
        {
            // Set the authorization header with the Bearer token
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            // Call SePay API to get bank accounts
            var bankAccountsEndpoint = $"{_settings.ApiBaseUrl}bank-accounts";
            var response = await _httpClient.GetAsync(bankAccountsEndpoint);
            
            // Handle errors
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get bank accounts. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                throw new Exception($"Failed to get bank accounts: {response.StatusCode}");
            }

            // Parse and return the response
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var bankAccounts = JsonConvert.DeserializeObject<BankAccountListResponseDto>(jsonResponse);
            
            if (bankAccounts == null)
            {
                throw new Exception("Failed to deserialize bank accounts response");
            }
            
            return bankAccounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bank accounts from SePay");
            throw;
        }
    }
}