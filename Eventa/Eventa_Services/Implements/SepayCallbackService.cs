using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Eventa_Services.Implements;

public class SepayCallbackService : ISepayCallbackService
{
    private readonly SepaySettings _settings;
    private readonly ILogger<SepayCallbackService> _logger;
    
    public SepayCallbackService(IOptions<SepaySettings> settings, ILogger<SepayCallbackService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> ValidateCallbackAsync(SepayCallbackDto callbackData)
    {
        try
        {
            // Verify that the callback is from SePay by checking the signature
            // The actual signature validation would depend on SePay's documentation
            // This is a simplified example
            
            var dataToSign = $"{callbackData.OrderCode}{callbackData.Status}{callbackData.Amount}{_settings.ClientSecret}";
            var computedSignature = GenerateSignature(dataToSign);
            
            // Compare the computed signature with the one received from SePay
            return computedSignature == callbackData.Signature;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating SePay callback");
            return false;
        }
    }

    public async Task<string> ProcessCallbackAsync(SepayCallbackDto callbackData)
    {
        try
        {
            // First validate the callback
            var isValid = await ValidateCallbackAsync(callbackData);
            if (!isValid)
            {
                _logger.LogWarning("Invalid callback data received from SePay: {OrderCode}", callbackData.OrderCode);
                return "INVALID_SIGNATURE";
            }
            
            // Update transaction status in your system
            var updated = await UpdateTransactionStatusAsync(callbackData.OrderCode, callbackData.Status);
            if (!updated)
            {
                _logger.LogWarning("Failed to update transaction status for order: {OrderCode}", callbackData.OrderCode);
                return "INTERNAL_ERROR";
            }
            
            // Additional processing based on payment status
            if (callbackData.Status == "success")
            {
                // Process successful payment
                _logger.LogInformation("Payment successful for order: {OrderCode}", callbackData.OrderCode);
                // TODO: Add your business logic for successful payments
            }
            else if (callbackData.Status == "failed")
            {
                // Process failed payment
                _logger.LogInformation("Payment failed for order: {OrderCode}", callbackData.OrderCode);
                // TODO: Add your business logic for failed payments
            }
            
            return "OK";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay callback for order: {OrderCode}", callbackData.OrderCode);
            return "INTERNAL_ERROR";
        }
    }

    public async Task<bool> UpdateTransactionStatusAsync(string orderCode, string status)
    {
        try
        {
            // In a real implementation, this would update the payment status in your database
            // For now, this is a placeholder
            
            _logger.LogInformation("Updating transaction status for order {OrderCode} to {Status}", orderCode, status);
            
            // TODO: Update transaction status in your database
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction status for order: {OrderCode}", orderCode);
            return false;
        }
    }
    
    private string GenerateSignature(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.ClientSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}