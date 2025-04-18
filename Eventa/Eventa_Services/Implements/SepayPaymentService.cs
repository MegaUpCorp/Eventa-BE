using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs;
using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Eventa_Services.Implements;

public class SepayPaymentService: ISepayService
{
    private readonly ISepayAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly SepaySettings _settings;
    private readonly OrderDAO _orderDAO;
    private readonly TransactionDAO _transactionDAO;
    private readonly ILogger<SepayPaymentService> _logger;
    private readonly IEventRepository  _eventRepository;
    private readonly SubscriptionPlanDAO _subscriptionPlanDAO;

    public SepayPaymentService(
        ISepayAuthService authService, 
        IOptions<SepaySettings> settings,
        OrderDAO orderDAO,

        TransactionDAO transactionDAO,
        ILogger<SepayPaymentService> logger,
        IEventRepository eventRepository,SubscriptionPlanDAO subscriptionPlanDAO)
    {
        _authService = authService;
        _settings = settings.Value;
        _httpClient = new HttpClient();
        _orderDAO = orderDAO;
        _transactionDAO = transactionDAO;
        _logger = logger;
        _eventRepository = eventRepository;
        _subscriptionPlanDAO = subscriptionPlanDAO;
    }
    public async Task<(string QrUrl, Order CreatedOrder)> GenerateSePayQrUrlAsync(EventDTO eve)
    {
        // Lấy thông tin sự kiện từ DB
        var ev = await _eventRepository.GetById(eve.EventId);

        string account;
        string bank;
        double price;
        string title;
        string orderType;
        Guid? eventId = null;
        Guid? subscriptionPlanId = null;

        if (ev != null)
        {
            // Trường hợp là sự kiện
            account = ev.BankAcc.acc;
            bank = ev.BankAcc.bank;
            price = ev.Price;
            title = ev.Title;
            orderType = "Event";
            eventId = ev.Id;
        }
        else
        {
            // Trường hợp không phải sự kiện -> giả sử lấy từ Subscription
            var sub = await _subscriptionPlanDAO.GetAsync(eve.EventId);
            if (sub == null)
                throw new Exception($"Không tìm thấy sự kiện hoặc subscription với ID = {eve.EventId}");

            account = sub.BankAcc.acc;
            bank = sub.BankAcc.bank;
            price = sub.MonthlyPrice;
            title = sub.PlanName;
            orderType = "Subscription";
            subscriptionPlanId = eve.EventId;
        }

        var newOrder = new Order
        {
            EventId = eventId,
            SubscriptionPlanId = subscriptionPlanId,
            Total = price,
            PaymentStatus = "Unpaid",
            Name = title,
            OrderType = orderType,
            PaymentMethod = bank,
            CreatedAt = DateTime.UtcNow
        };

        await _orderDAO.CreateOrderAsync(newOrder);

        var amount = (int)price;
        var description = $"ORDER_{newOrder.Id:N}";
        var template = "";
        var download = "false";

        var qrUrl = $"https://qr.sepay.vn/img?acc={account}&bank={bank}&amount={amount}&des={description}&template={template}&download={download}";

        return (qrUrl, newOrder);
    }

    public async Task HandleWebhookAsync(SepayWebhookPayload payload)
    {
        try
        {
            _logger.LogInformation("Nhận Webhook từ SePay: {@payload}", payload);

            if (payload.transferType != "in")
            {
                _logger.LogInformation("Bỏ qua giao dịch vì không phải tiền vào");
                return;
            }

            // Parse eventId từ nội dung giao dịch
            var match = Regex.Match(payload.content, @"ORDER[_\.]?([a-fA-F0-9]{32})");
            if (!match.Success)
            {
                _logger.LogWarning("Không tìm thấy OrderId trong nội dung giao dịch: {Content}", payload.content);
                return;
            }

            var orderId = Guid.ParseExact(match.Groups[1].Value,"N");
            var order = await _orderDAO.GetOrderByIdAsync(orderId.ToString());
            if (order == null)
            {
                _logger.LogWarning("Không tìm thấy Order với ID: {OrderId}", orderId);
                return;
            }
            order.PaymentStatus = "Paid";
            await _orderDAO.UpdateAsync(order);

            // Ghi nhận giao dịch vào DB
            var transaction = new Transaction
            {
                EventId = (Guid)order.EventId,
                SubscriptionPlanId = order.SubscriptionPlanId,
                TransactionDate = DateTime.Parse(payload.transactionDate),
                Amount = payload.transferAmount,
                AmountIn = payload.transferType == "in" ? payload.transferAmount : 0,
                AmountOut = payload.transferType == "out" ? payload.transferAmount : 0,
                Accumulated = payload.accumulated,
                AccountNumber = payload.accountNumber,
                SubAccount = payload.subAccount ?? string.Empty,
                Code = payload.code ?? string.Empty,
                Bank = payload.gateway,
                ReferenceCode = payload.referenceCode,
                OrderId = orderId,
                Description = payload.description,
                TransactionContent = payload.content
            };

            await _transactionDAO.CreateTransactionAsync(transaction);

            _logger.LogInformation("Đã ghi nhận giao dịch cho sự kiện: {EventId}", transaction.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi xử lý webhook từ SePay");
            throw;
        }
    }
    public async Task CancelExpiredOrdersAsync()
    {
        var expiredOrders = await _orderDAO.GetUnpaidOrdersOlderThan(TimeSpan.FromMinutes(5));

        foreach (var order in expiredOrders)
        {
            order.PaymentStatus = "Cancelled";
            await _orderDAO.UpdateAsync(order);
        }
    }
    public async Task RefundOrderAsync(Guid orderId, string reason)
    {
        var order = await _orderDAO.GetAsync(orderId);
        if (order == null || order.PaymentStatus != "Paid")
            throw new Exception("Không thể hoàn tiền cho đơn không tồn tại hoặc chưa thanh toán.");

        order.PaymentStatus = "Refunded";
        order.RefundDate = DateTime.UtcNow;
        order.RefundReason = reason;
        order.IsManualRefund = true;

        await _orderDAO.UpdateAsync(order);
    }
    public async Task<string> CheckStatusOrder(Guid orderId)
    {
        var order = await _orderDAO.GetAsync(orderId);
        return order.PaymentStatus;

    }

    public async Task<Transaction> CreateTransaction(SepayWebhookPayload payload)
    {
        try
        {
            var match = Regex.Match(payload.content, @"EVT[_]?([a-fA-F0-9]{32})");
            if (!match.Success)
            {
                _logger.LogWarning("Không tìm thấy EventId trong nội dung giao dịch: {Content}", payload.content);
            }

            var eventId = Guid.Parse(match.Groups[1].Value);
            var ev = await _eventRepository.GetById(eventId);
            if (ev == null)
            {
                _logger.LogWarning("Không tìm thấy Event với ID: {EventId}", eventId);
            }
            var transaction = new Transaction
            {
                EventId = eventId,
                TransactionDate = DateTime.Parse(payload.transactionDate),
                Amount = payload.transferAmount,
                AmountIn = payload.transferType == "in" ? payload.transferAmount : 0,
                AmountOut = payload.transferType == "out" ? payload.transferAmount : 0,
                Accumulated = payload.accumulated,
                AccountNumber = payload.accountNumber,
                SubAccount = payload.subAccount ?? string.Empty,
                Code = payload.code ?? string.Empty,
                Bank = payload.gateway,
                ReferenceCode = payload.referenceCode,
                Description = payload.description,
                TransactionContent = payload.content
            };

            // Ghi nhận giao dịch vào DB
            return await _transactionDAO.CreateTransactionAsync(transaction);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi ghi nhận giao dịch vào DB");
            throw;
        }
    }
    public async Task<List<Transaction>> GetAllTransactions()
    {
        try
        {
            var transactions = await _transactionDAO.GetAllTransactionsAsync();
            return transactions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi lấy danh sách giao dịch");
            throw;
        }
    }
    public class SepayWebhookPayload
    {
        public long id { get; set; }
        public string gateway { get; set; }
        public string transactionDate { get; set; }
        public string? accountNumber { get; set; }
        public string? code { get; set; }
        public string content { get; set; }
        public string transferType { get; set; }
        public int transferAmount { get; set; }
        public long accumulated { get; set; }
        public string? subAccount { get; set; }
        public string referenceCode { get; set; }
        public string description { get; set; }
    }
    public async Task<SubscriptionPlan> CreateSubscriptionPlan(SubscriptionPlan plan)
    {
        var newPlan = new SubscriptionPlan
        {
            PlanName = plan.PlanName,
            MonthlyPrice = plan.MonthlyPrice,
            IsBilledAnnually = plan.IsBilledAnnually,
            AnnualDiscountPercent = plan.AnnualDiscountPercent,
            ButtonText = plan.ButtonText,
            IncludesFreePlan = plan.IncludesFreePlan,
            MaxInvitationsPerWeek = plan.MaxInvitationsPerWeek,
            TaxCollectionEnabled = plan.TaxCollectionEnabled,
            CheckinManagement = plan.CheckinManagement,
            CustomEventURL = plan.CustomEventURL,
            CollectFullName = plan.CollectFullName,
            DefaultAdminCount = plan.DefaultAdminCount,
            ExtraAdminPurchaseable = plan.ExtraAdminPurchaseable,
            ZapierAutomation = plan.ZapierAutomation,
            APIAccess = plan.APIAccess,
            Features = plan.Features
        };
        await _subscriptionPlanDAO.AddAsync(newPlan);
        return newPlan;
    }



    public async Task<string> CreatePaymentAsync(PaymentRequestDto paymentDto)
    {
        try
        {
            // Create or update an order record in our database
            var order = await CreateOrderFromPaymentRequestAsync(paymentDto);
            
            // Generate signature for the payment request
            paymentDto.signature = GenerateSignature(paymentDto);
            
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
                        
                        // Update order status to indicate failure
                        await _orderDAO.UpdateOrderStatusAsync(order.Id, "PAYMENT_FAILED");
                        
                        throw new Exception($"Payment request failed after token refresh. Status: {response.StatusCode}, Error: {errorContent}");
                    }
                }
                else
                {
                    // Update order status to indicate failure
                    await _orderDAO.UpdateOrderStatusAsync(order.Id, "PAYMENT_FAILED");
                    
                    // For other errors, throw an exception
                    throw new Exception($"Payment request failed. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }

            // Get the payment response
            var result = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<dynamic>(result);
            
            // Update the order with the payment URL and change status to PENDING
            if (paymentResponse != null)
            {
                // Extract payment URL or transaction ID from the response and update the order
                await _orderDAO.UpdateOrderStatusAsync(order.Id, "PENDING");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment with SePay");
            throw;
        }
    }
    
    private async Task<Order> CreateOrderFromPaymentRequestAsync(PaymentRequestDto paymentDto)
    {
        // Check if an order already exists with this order code
        //var existingOrder = await _orderDAO.GetOrderByOrderCodeAsync(paymentDto.order_id);
        //if (existingOrder != null)
        //{
        //    // If the order exists but is in a failed or cancelled state, we can update it for a retry
        //    if (existingOrder.Status == "PAYMENT_FAILED" || existingOrder.Status == "CANCELLED")
        //    {
        //        existingOrder.Status = "CREATED";
        //        existingOrder.UpdDate = DateTime.UtcNow;
        //        existingOrder.PaymentMethod = "SePay";
        //        existingOrder.Total = decimal.Parse(paymentDto.amount);
        //        //existingOrder.CustomerName = paymentDto.customer_name;
        //        //existingOrder.CustomerEmail = paymentDto.customer_email;
        //        //existingOrder.CustomerPhone = paymentDto.customer_phone;

        //        await _orderDAO.UpdateOrderAsync(existingOrder);
        //        return existingOrder;
        //    }

        //    // Otherwise, return the existing order
        //    return existingOrder;
        //}

        //// Create a new order record
        var order = new Order
        {
            // OrderCode = paymentDto.order_id,
            Total = double.Parse(paymentDto.amount),
            Status = "CREATED",
            PaymentMethod = "SePay",
            //CustomerName = paymentDto.customer_name,
            //CustomerEmail = paymentDto.customer_email,
            //CustomerPhone = paymentDto.customer_phone,
            Note = paymentDto.order_info
        };

        // Save the order to the database
        await _orderDAO.CreateOrderAsync(order);

        return order;
    }
    
    public async Task<PaymentStatusResponseDto> CheckPaymentStatusAsync(string orderCode)
    {
        try
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
            
            // Update order status based on response
            //var order = await _orderDAO.GetOrderByOrderCodeAsync(orderCode);
            //if (order != null && statusResponse.Status != null)
            //{
            //    string newStatus;
                
            //    switch (statusResponse.Status.ToLower())
            //    {
            //        case "succeeded":
            //        case "success":
            //            newStatus = "PAID";
            //            break;
            //        case "pending":
            //            newStatus = "PENDING";
            //            break;
            //        case "failed":
            //            newStatus = "PAYMENT_FAILED";
            //            break;
            //        case "cancelled":
            //        case "canceled":
            //            newStatus = "CANCELLED";
            //            break;
            //        default:
            //            newStatus = "UNKNOWN";
            //            break;
            //    }
                
            //    await _orderDAO.UpdateOrderStatusAsync(order.Id, newStatus);
            //}
            
            return statusResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for order: {OrderCode}", orderCode);
            throw;
        }
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
        
        // Update order status to refunded
        //var order = await _orderDAO.GetOrderByOrderCodeAsync(refundRequestDto.OrderCode);
        //if (order != null && refundResponse.Status == "success")
        //{
        //    await _orderDAO.UpdateOrderStatusAsync(order.Id, "REFUNDED");
            
        //    // Create a transaction record for the refund
        //    var transaction = new Transaction
        //    {
        //        Gateway = "SePay",
        //        TransactionDate = DateTime.UtcNow,
        //        AmountOut = refundRequestDto.Amount,
        //        Code = refundRequestDto.OrderCode,
        //        TransactionContent = $"Refund for order {refundRequestDto.OrderCode}",
        //        ReferenceNumber = refundResponse.TransactionId,
        //       // Body = JsonConvert.SerializeObject(refundResponse)
        //    };
            
        //    await _transactionDAO.CreateTransactionAsync(transaction);
        //}
        
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

        // Update order status to cancelled if successful
        //if (response.IsSuccessStatusCode)
        //{
        //    var order = await _orderDAO.GetOrderByOrderCodeAsync(orderCode);
        //    if (order != null)
        //    {
        //        await _orderDAO.UpdateOrderStatusAsync(order.Id, "CANCELLED");
        //    }
        //}

        // Return success status based on HTTP response
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Generates a signature for SePay payment request according to their documentation
    /// </summary>
    private string GenerateSignature(PaymentRequestDto paymentDto)
    {
        // Build the string to be signed according to SePay requirements
        // The format is typically: amount|currency|order_id|order_info|return_url|notify_url|client_secret
        var dataToSign = $"{paymentDto.amount}|{paymentDto.currency}|{paymentDto.order_id}|{paymentDto.order_info}|{paymentDto.return_url}|{paymentDto.notify_url}|{_settings.ClientSecret}";
        
        // Compute the signature using SHA256
        using var sha256 = SHA256.Create();
        var signatureBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        
        // Convert the signature to a hexadecimal string
        var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
        
        return signature;
    }

    /// <summary>
    /// Verifies the signature in a callback notification from SePay
    /// </summary>
    public bool VerifySignature(SepayCallbackDto callbackData)
    {
        // Build the string to verify according to SePay documentation
        // Format: order_id|amount|status|client_secret
        var dataToVerify = $"{callbackData.OrderCode}|{callbackData.Amount}|{callbackData.Status}|{_settings.ClientSecret}";
        
        // Compute the expected signature
        using var sha256 = SHA256.Create();
        var signatureBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify));
        var expectedSignature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
        
        // Compare the computed signature with the one received from SePay
        return string.Equals(expectedSignature, callbackData.Signature, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<PaymentStatusResponseDto>> GetAllPaymentsAsync()
    {
        // Fetch all orders from the database
        var orders = await _orderDAO.GetAllOrdersAsync();

        // Map orders to PaymentStatusResponseDto
        var payments = orders.Select(order => new PaymentStatusResponseDto
        {
          //  OrderCode = order.OrderCode,
            Status = order.Status,
            Amount = (decimal)order.Total,
            Currency = "USD", // Replace with actual currency if available
            PaymentTime = order.UpdDate, // Assuming UpdatedAt represents payment time
            PaymentMethod = order.PaymentMethod,
          //  TransactionId = order.TransactionId
        }).ToList();

        return payments;
    }
}
