using Eventa_BusinessObject.DTOs;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Eventa_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ISepayService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ISepayService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllPayments()
    {
        try
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(new
            {
                success = true,
                data = payments
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all payments");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
    {
        try
        {
            // Validate the request
            var validationError = ValidatePaymentRequest(request);
            if (!string.IsNullOrEmpty(validationError))
                return BadRequest(new { success = false, message = validationError });

            // Set defaults if not provided
            request.currency ??= "VND";
            request.language ??= "vn";
            request.order_info ??= $"Payment for order {request.order_id}";

            // Create payment via SePay
            var resultJson = await _paymentService.CreatePaymentAsync(request);

            if (string.IsNullOrEmpty(resultJson))
            {
                _logger.LogWarning("Received null or empty response from SePay for order: {OrderId}", request.order_id);
                return StatusCode(500, new { success = false, message = "Failed to create payment" });
            }

            // Log the response
            _logger.LogInformation("SePay payment response: {Response}", resultJson);

            // Parse the response
            var paymentResponse = JsonConvert.DeserializeObject<dynamic>(resultJson);

            // Return the payment URL and other details
            return Ok(new
            {
                success = true,
                message = "Payment created successfully",
                data = paymentResponse
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SePay payment");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
    private string ValidatePaymentRequest(PaymentRequestDto request)
    {
        if (string.IsNullOrEmpty(request.amount))
            return "Amount is required";
        if (string.IsNullOrEmpty(request.order_id))
            return "Order ID is required";
        if (string.IsNullOrEmpty(request.return_url))
            return "Return URL is required";
        return null;
    }

    [HttpGet("status/{orderId}")]
    public async Task<IActionResult> CheckPaymentStatus(string orderId)
    {
        try
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest(new { success = false, message = "Order ID is required" });

            var status = await _paymentService.CheckPaymentStatusAsync(orderId);

            // Log the status response
            _logger.LogInformation("Payment status for order {OrderId}: {Status}", orderId, status);

            return Ok(new
            {
                success = true,
                data = status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for order {OrderId}", orderId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
    
    [HttpPost("refund")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.OrderCode))
                return BadRequest(new { success = false, message = "Order code is required" });
            
            var refundResult = await _paymentService.ProcessRefundAsync(request);
            
            return Ok(new {
                success = true,
                message = "Refund processed successfully",
                data = refundResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
    
    [HttpPost("cancel/{orderId}")]
    public async Task<IActionResult> CancelPayment(string orderId)
    {
        try
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest(new { success = false, message = "Order ID is required" });
            
            var result = await _paymentService.CancelPaymentAsync(orderId);
            
            return Ok(new {
                success = true,
                message = result ? "Payment cancelled successfully" : "Failed to cancel payment"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment for order {OrderId}", orderId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}