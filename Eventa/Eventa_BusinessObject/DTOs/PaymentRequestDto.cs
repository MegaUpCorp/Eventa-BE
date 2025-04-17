namespace Eventa_BusinessObject.DTOs;

public class PaymentRequestDto
{
    public string amount { get; set; }
    public string currency { get; set; }
    public string order_id { get; set; }
    public string return_url { get; set; }
}