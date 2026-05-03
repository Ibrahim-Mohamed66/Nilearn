using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Payments.Commands
{
    internal sealed class ProcessPaymentWebhookCommandHandler
        : IRequestHandler<ProcessPaymentWebhookCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymentWebhookCommandHandler> _logger;


        public ProcessPaymentWebhookCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ProcessPaymentWebhookCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(
            ProcessPaymentWebhookCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validate required identifiers
            if (request.MerchantReferenceId is null)
            {
                _logger.LogWarning("Webhook received without MerchantReferenceId. TransactionId: {TransactionId}",
                    request.TransactionId);

                return Result<string>.FailureResponse(message: "Missing merchant reference id");
            }

            // 2. Load payment by merchant reference (your internal key)
            var payment = await _unitOfWork.PaymentRepository
                .GetByMerchantReferenceAsync(request.MerchantReferenceId, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Payment not found. MerchantRef: {Ref}, TransactionId: {TransactionId}",
                    request.MerchantReferenceId,
                    request.TransactionId);

                return Result<string>.FailureResponse(message:"Payment not found");
            }

            // 3. Idempotency check (VERY IMPORTANT for webhooks)
            if (payment.IsFinalized)
            {
                _logger.LogInformation("Duplicate webhook ignored. PaymentId: {PaymentId}", payment.Id);
                return Result<string>.SuccessResponse(message: "Already processed");
            }

            // 4. Apply business logic
            if (request.IsSuccess)
            {
                payment.MarkAsSucceeded(
                    transactionId: request.TransactionId.ToString(),
                    orderId: request.OrderId.ToString()
                );

                _logger.LogInformation(
                    "Payment succeeded. PaymentId: {PaymentId}, TransactionId: {TransactionId}",
                    payment.Id,
                    request.TransactionId);
            }
            else
            {
                payment.MarkAsFailed(
                    transactionId: request.TransactionId.ToString(),
                    orderId: request.OrderId.ToString()
                );
                

                _logger.LogWarning(
                    "Payment failed. PaymentId: {PaymentId}, TransactionId: {TransactionId}",
                    payment.Id,
                    request.TransactionId);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.SuccessResponse(message:"Webhook processed successfully");
        }
    }
}