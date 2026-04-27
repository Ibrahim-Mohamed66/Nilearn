using Nilearn.Domain.Enums;

namespace Nilearn.Domain.Exceptions;

public sealed class PaymentAlreadyFinalizedException : InvalidOperationException
{
    public PaymentAlreadyFinalizedException(int paymentId, PaymentStatus currentStatus, string? incomingTransactionId)
        : base(
            $"Payment {paymentId} is already finalized with status '{currentStatus}'. " +
            $"Incoming transactionId: '{incomingTransactionId ?? "null"}'.")
    {
    }
}