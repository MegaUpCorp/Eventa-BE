using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Eventa_Services.Implements;

public class SepayCallbackService : ISepayCallbackService
{
    private readonly SepaySettings _settings;
    private readonly ILogger<SepayCallbackService> _logger;
    private readonly OrderDAO _orderDAO;
    private readonly TransactionDAO _transactionDAO;
    
    public SepayCallbackService(
        IOptions<SepaySettings> settings, 
        ILogger<SepayCallbackService> logger,
        OrderDAO orderDAO,
        TransactionDAO transactionDAO)
    {
        _settings = settings.Value;
        _logger = logger;
        _orderDAO = orderDAO;
        _transactionDAO = transactionDAO;
    }

    public Task<bool> ValidateCallbackAsync(SepayCallbackDto callbackData)
    {
        try
        {
            // Verify that the callback is from SePay by checking the signature
            var dataToSign = $"{callbackData.OrderCode}{callbackData.Status}{callbackData.Amount}{_settings.ClientSecret}";
            var computedSignature = GenerateSignature(dataToSign);
            
            // Compare the computed signature with the one received from SePay
            return Task.FromResult(computedSignature == callbackData.Signature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating SePay callback");
            return Task.FromResult(false);
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

            // Update order and transaction status in our system
            var updated = await UpdateTransactionStatusAsync(callbackData);
            if (!updated)
            {
                _logger.LogWarning("Failed to update transaction status for order: {OrderCode}", callbackData.OrderCode);
                return "INTERNAL_ERROR";
            }

            // Additional processing based on payment status
            if (callbackData.Status?.ToLower() == "success" || callbackData.Status?.ToLower() == "succeeded")
            {
                // Process successful payment
                _logger.LogInformation("Payment successful for order: {OrderCode}", callbackData.OrderCode);

                // Update order status to confirm the payment was successful
                var order = await _orderDAO.GetOrderByOrderCodeAsync(callbackData.OrderCode);
                if (order != null)
                {
                    await _orderDAO.UpdateOrderStatusAsync(order.Id, "PAID");
                }
            }
            else if (callbackData.Status?.ToLower() == "failed")
            {
                // Process failed payment
                _logger.LogInformation("Payment failed for order: {OrderCode}", callbackData.OrderCode);

                // Update order status to indicate payment failure
                var order = await _orderDAO.GetOrderByOrderCodeAsync(callbackData.OrderCode);
                if (order != null)
                {
                    await _orderDAO.UpdateOrderStatusAsync(order.Id, "PAYMENT_FAILED");
                }
            }
            else
            {
                _logger.LogWarning("Unexpected payment status: {Status} for order: {OrderCode}", callbackData.Status, callbackData.OrderCode);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SePay callback for order: {OrderCode}", callbackData.OrderCode);
            return "INTERNAL_ERROR";
        }
    }

    public async Task<bool> UpdateTransactionStatusAsync(SepayCallbackDto callbackData)
    {
        try
        {
            // Find the order by order code
            var order = await _orderDAO.GetOrderByOrderCodeAsync(callbackData.OrderCode);
            if (order == null)
            {
                _logger.LogWarning("Order not found for order code: {OrderCode}", callbackData.OrderCode);
                return false;
            }

            // Create or update the transaction record
            if (string.IsNullOrEmpty(order.TransactionId))
            {
                // Create a new transaction record
                var transaction = new Transaction
                {
                    Gateway = "SePay",
                    TransactionDate = DateTime.UtcNow,
                    AmountIn = callbackData.Amount,
                    Code = callbackData.OrderCode,
                    TransactionContent = $"Payment for order {callbackData.OrderCode}",
                    ReferenceNumber = callbackData.TransactionId,
                    Body = JsonConvert.SerializeObject(callbackData)
                };

                var createdTransaction = await _transactionDAO.CreateTransactionAsync(transaction);

                // Update the order with the transaction reference
                order.TransactionId = createdTransaction.Id;
                order.Status = MapPaymentStatus(callbackData.Status);
                order.UpdDate = DateTime.UtcNow;

                await _orderDAO.UpdateOrderAsync(order);
            }
            else
            {
                // Update the existing transaction
                var transaction = await _transactionDAO.GetTransactionByIdAsync(order.TransactionId);
                if (transaction != null)
                {
                    transaction.ReferenceNumber = callbackData.TransactionId;
                    transaction.Body = JsonConvert.SerializeObject(callbackData);

                    await _transactionDAO.UpdateTransactionAsync(transaction);

                    // Update order status
                    order.Status = MapPaymentStatus(callbackData.Status);
                    order.UpdDate = DateTime.UtcNow;

                    await _orderDAO.UpdateOrderAsync(order);
                }
                else
                {
                    _logger.LogWarning("Transaction not found for ID: {TransactionId}", order.TransactionId);
                    return false;
                }
            }

            _logger.LogInformation("Successfully updated transaction and order status for order {OrderCode}", callbackData.OrderCode);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction status for order: {OrderCode}", callbackData.OrderCode);
            return false;
        }
    }

    private string MapPaymentStatus(string sepayStatus)
    {
        return sepayStatus.ToLower() switch
        {
            "success" or "succeeded" => "PAID",
            "pending" => "PENDING",
            "failed" => "PAYMENT_FAILED",
            "canceled" or "cancelled" => "CANCELLED",
            _ => "UNKNOWN"
        };
    }
    
    private string GenerateSignature(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.ClientSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}