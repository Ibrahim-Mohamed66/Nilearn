namespace Nilearn.Application.Common;

public class PaymentWebhookResult
{
    public bool IsSuccess { get; set; }

    public long TransactionId { get; set; } = default!;
    public long OrderId { get; set; } = default!;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;

    public string? ResponseCode { get; set; }
    public string? Message { get; set; }

    // Optional (only if you fix mapping)
    public string? MerchantReferenceId { get; set; }
    public string? EnrollmentId { get; set; }
}