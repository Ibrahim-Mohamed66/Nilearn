using Nilearn.Domain.Enums;
using Nilearn.Domain.Exceptions;
namespace Nilearn.Domain.Entities;

public class Payment : BaseEntity
{
    public int EnrollmentId { get;  private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; } = Currency.EGP;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    // Paymob-specific
    public string? PaymobIntentionId { get; private set; }
    public string? PaymobTransactionId { get; private set; }  
    public string? PaymobOrderId { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string MerchantReferenceId {  get; private set; }

    public byte[] Version { get; private set; }
    public Enrollment? Enrollment { get; private set; }

    public Payment(int enrollmentId, decimal amount, Currency currency = Currency.EGP)
    {
        EnrollmentId = enrollmentId;
        Amount = amount;
        Currency = currency;
        MerchantReferenceId = $"enroll_{enrollmentId}_{Guid.NewGuid():N}";
    }

    protected Payment() { } // For EF Core

    public void MarkAsSucceeded(string transactionId, string orderId, string intentionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(intentionId);

        if (IsSuccessful && PaymobTransactionId == transactionId)
            return;

        if (IsFinalized)
            throw new PaymentAlreadyFinalizedException(Id, Status, transactionId);


        Status = PaymentStatus.Success;
        PaymobTransactionId = transactionId;
        PaymobOrderId = orderId;
        PaymobIntentionId = intentionId;
        PaidAt = DateTime.UtcNow;
       
    }

    public void MarkAsFailed(string? transactionId = null, string? orderId = null, string? intentionId = null)
    {
        if (IsFailed && transactionId == PaymobTransactionId)
            return;

        if (IsFinalized)
            throw new PaymentAlreadyFinalizedException(Id, Status, transactionId);
       

        Status = PaymentStatus.Failed;
        PaymobTransactionId = transactionId ?? PaymobTransactionId;
        PaymobOrderId = orderId ?? PaymobOrderId;
        PaymobIntentionId = intentionId ?? PaymobIntentionId;

    }
    public bool IsSuccessful => Status == PaymentStatus.Success;
    public bool IsPending => Status == PaymentStatus.Pending;
    public bool IsFailed => Status == PaymentStatus.Failed;
    public bool IsFinalized =>
    Status == PaymentStatus.Success || Status == PaymentStatus.Failed;

}
