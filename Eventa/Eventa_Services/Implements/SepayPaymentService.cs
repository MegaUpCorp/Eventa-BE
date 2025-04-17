using System.Net.Http.Headers;
using System.Net.Http.Json;
using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Eventa_Services.Implements;

public class SepayPaymentService: ISepayService
{
    private readonly ISepayAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly SepaySettings _settings;

    public SepayPaymentService(ISepayAuthService authService, IOptions<SepaySettings> settings)
    {
        _authService = authService;
        _settings = settings.Value;
        _httpClient = new HttpClient();
    }

    public async Task<string> CreatePaymentAsync(PaymentRequestDto paymentDto)
    {
        // Get the access token using the auth service
        var accessToken = await _authService.GetAccessTokenAsync();
        
        // Set the authorization header with the Bearer token
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Create the payment request to SePay API
        var paymentEndpoint = $"{_settings.ApiBaseUrl}/payment";
        var response = await _httpClient.PostAsJsonAsync(paymentEndpoint, paymentDto);
        
        // Handle any errors
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // If token expired (401), try to refresh token and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Clear old authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Get a fresh token (the GetAccessTokenAsync will handle expiry internally)
                accessToken = await _authService.GetAccessTokenAsync();
                
                // Set the new authorization header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Retry the request
                response = await _httpClient.PostAsJsonAsync(paymentEndpoint, paymentDto);
                
                // If still unsuccessful, throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Payment request failed after token refresh. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            else
            {
                // For other errors, throw an exception
                throw new Exception($"Payment request failed. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }

        // Return the payment response
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
    
    public async Task<PaymentStatusResponseDto> CheckPaymentStatusAsync(string orderCode)
    {
        // Get the access token using the auth service
        var accessToken = await _authService.GetAccessTokenAsync();
        
        // Set the authorization header with the Bearer token
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Call SePay API to check payment status
        var statusEndpoint = $"{_settings.ApiBaseUrl}/payment/status/{orderCode}";
        var response = await _httpClient.GetAsync(statusEndpoint);
        
        // Handle errors with retry for unauthorized
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Clear old authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Get a fresh token
                accessToken = await _authService.GetAccessTokenAsync();
                
                // Set the new authorization header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Retry the request
                response = await _httpClient.GetAsync(statusEndpoint);
                
                // If still unsuccessful, throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Payment status check failed after token refresh. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            else
            {
                // For other errors, throw an exception
                throw new Exception($"Payment status check failed. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }

        // Parse and return the status response
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var statusResponse = JsonConvert.DeserializeObject<PaymentStatusResponseDto>(jsonResponse);
        
        if (statusResponse == null)
        {
            throw new Exception("Failed to deserialize payment status response");
        }
        
        return statusResponse;
    }
    
    public async Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto)
    {
        // Get the access token using the auth service
        var accessToken = await _authService.GetAccessTokenAsync();
        
        // Set the authorization header with the Bearer token
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Create the refund request to SePay API
        var refundEndpoint = $"{_settings.ApiBaseUrl}/payment/refund";
        var response = await _httpClient.PostAsJsonAsync(refundEndpoint, refundRequestDto);
        
        // Handle errors with retry for unauthorized
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Clear old authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Get a fresh token
                accessToken = await _authService.GetAccessTokenAsync();
                
                // Set the new authorization header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Retry the request
                response = await _httpClient.PostAsJsonAsync(refundEndpoint, refundRequestDto);
                
                // If still unsuccessful, throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Refund request failed after token refresh. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            else
            {
                // For other errors, throw an exception
                throw new Exception($"Refund request failed. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }

        // Parse and return the refund response
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var refundResponse = JsonConvert.DeserializeObject<RefundResponseDto>(jsonResponse);
        
        if (refundResponse == null)
        {
            throw new Exception("Failed to deserialize refund response");
        }
        
        return refundResponse;
    }
    
    public async Task<bool> CancelPaymentAsync(string orderCode)
    {
        // Get the access token using the auth service
        var accessToken = await _authService.GetAccessTokenAsync();
        
        // Set the authorization header with the Bearer token
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Create the cancel request to SePay API
        var cancelEndpoint = $"{_settings.ApiBaseUrl}/payment/cancel/{orderCode}";
        var response = await _httpClient.PostAsync(cancelEndpoint, null);
        
        // Handle errors with retry for unauthorized
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Only retry on unauthorized
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Clear old authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Get a fresh token
                accessToken = await _authService.GetAccessTokenAsync();
                
                // Set the new authorization header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Retry the request
                response = await _httpClient.PostAsync(cancelEndpoint, null);
                
                // If still unsuccessful, throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Cancel payment request failed after token refresh. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            else
            {
                // For other errors, throw an exception
                throw new Exception($"Cancel payment request failed. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }

        // Return success status based on HTTP response
        return response.IsSuccessStatusCode;
    }
}
