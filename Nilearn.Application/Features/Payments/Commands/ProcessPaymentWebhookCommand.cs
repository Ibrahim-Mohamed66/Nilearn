using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Payments.Commands;

public record ProcessPaymentWebhookCommand(
    long TransactionId,
    long OrderId,
    bool IsSuccess,
    decimal Amount,
    string Currency,
    string? MerchantReferenceId,
    string? EnrollmentId
) : IRequest<Result<string>>;