using Eventa_BusinessObject.DTOs;

namespace Eventa_Services.Interfaces;

public interface ISepayCallbackService
{
    /// <summary>
    /// Xác thực callback từ SePay
    /// </summary>
    /// <param name="callbackData">Dữ liệu callback từ SePay</param>
    /// <returns>true nếu callback hợp lệ, false nếu không</returns>
    Task<bool> ValidateCallbackAsync(SepayCallbackDto callbackData);
    
    /// <summary>
    /// Xử lý callback từ SePay
    /// </summary>
    /// <param name="callbackData">Dữ liệu callback từ SePay</param>
    /// <returns>Kết quả xử lý</returns>
    Task<string> ProcessCallbackAsync(SepayCallbackDto callbackData);
    
    /// <summary>
    /// Cập nhật trạng thái giao dịch
    /// </summary>
    /// <param name="orderCode">Mã đơn hàng</param>
    /// <param name="status">Trạng thái thanh toán</param>
    /// <returns>true nếu cập nhật thành công, false nếu không</returns>
    Task<bool> UpdateTransactionStatusAsync(string orderCode, string status);
}