using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Domain.Entities;
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

                var instructorShare = payment.Amount * 0.8m; // Example: 80% to instructor
                var platformShare = payment.Amount - instructorShare;

                var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(payment.EnrollmentId, cancellationToken);
                if (enrollment is null)
                    return Result<string>.FailureResponse(message: "Enrollment not found");

                if (enrollment.Course is null)
                    return Result<string>.FailureResponse(message: "Course not found for enrollment");


                var instructorId = enrollment.Course.InstructorId;
                var instructorWallet = await _unitOfWork.InstructorWalletRepository.GetByInstructorIdAsync(instructorId, cancellationToken);
                if(instructorWallet is null)
                {
                    var instructorWalletEntity = new Domain.Entities.InstructorWallet
                    {
                        InstructorId = instructorId
                    };
                    instructorWalletEntity.Credit(instructorShare);
                    await _unitOfWork.InstructorWalletRepository.AddAsync(instructorWalletEntity, cancellationToken);
                }
                else
                {
                    instructorWallet.Credit(instructorShare);
                }
                var platformWallet = await _unitOfWork.PlatformWalletRepository.GetAsync(cancellationToken);
                if (platformWallet is null)
                {
                    var platformWalletEntity = new PlatformWallet();
                    platformWalletEntity.Credit(platformShare);
                    await _unitOfWork.PlatformWalletRepository.AddAsync(platformWalletEntity, cancellationToken);
                }
                else
                {
                    platformWallet.Credit(platformShare);
                }

                var InstructorTransaction = new WalletTransaction();
                InstructorTransaction.Credit(instructorId, instructorShare,
                    $"Earnings from payment {payment.Id} for {enrollment.Course.Title} for instructor {instructorId}",
                    payment.Id);

                var platformTransaction = new WalletTransaction();
                platformTransaction.Credit(
                    instructorId: null,
                    amount: platformShare,
                    description: $"Platform share from payment {payment.Id} for {enrollment.Course.Title} for instructor {instructorId}",
                    paymentId: payment.Id
                );
                

                await _unitOfWork.WalletTransactionRepository.AddAsync(InstructorTransaction, cancellationToken);
                await _unitOfWork.WalletTransactionRepository.AddAsync(platformTransaction, cancellationToken);
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