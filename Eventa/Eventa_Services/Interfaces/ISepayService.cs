using Eventa_BusinessObject.DTOs;
using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using static Eventa_Services.Implements.SepayPaymentService;

namespace Eventa_Services.Interfaces;

public interface ISepayService
{
    /// <summary>
    /// Tạo một thanh toán mới
    /// </summary>
    /// <param name="paymentDto">Thông tin thanh toán</param>
    /// <returns>URL redirect hoặc thông tin thanh toán</returns>
    Task<string> CreatePaymentAsync(PaymentRequestDto paymentDto);
    
    /// <summary>
    /// Kiểm tra trạng thái thanh toán
    /// </summary>
    /// <param name="orderCode">Mã đơn hàng</param>
    /// <returns>Thông tin trạng thái thanh toán</returns>
    Task<PaymentStatusResponseDto> CheckPaymentStatusAsync(string orderCode);
    
    /// <summary>
    /// Hoàn tiền cho giao dịch
    /// </summary>
    /// <param name="refundRequestDto">Thông tin hoàn tiền</param>
    /// <returns>Kết quả hoàn tiền</returns>
    Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto refundRequestDto);
    
    /// <summary>
    /// Hủy giao dịch thanh toán
    /// </summary>
    /// <param name="orderCode">Mã đơn hàng</param>
    /// <returns>Kết quả hủy giao dịch</returns>
    Task<bool> CancelPaymentAsync(string orderCode);
    bool VerifySignature(SepayCallbackDto callbackData);

    /// <summary>
    /// Retrieves all payments.
    /// </summary>
    /// <returns>A list of all payments.</returns>
    Task<List<PaymentStatusResponseDto>> GetAllPaymentsAsync();
    Task<(string QrUrl, Order CreatedOrder)> GenerateSePayQrUrlAsync(EventDTO eve, HttpContext httpContext);
    Task HandleWebhookAsync(SepayWebhookPayload payload);
    Task<Transaction> CreateTransaction(SepayWebhookPayload payload);
     Task<List<Transaction>> GetAllTransactions();
    Task CancelExpiredOrdersAsync();
    Task RefundOrderAsync(Guid orderId, string reason);
    Task<SubscriptionPlan> CreateSubscriptionPlan(SubscriptionPlan plan);
    Task<string> CheckStatusOrder(Guid orderId);
    Task<bool> CheckPremium(HttpContext httpContextm);
}