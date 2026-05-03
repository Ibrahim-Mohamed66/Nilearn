using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Domain.Events;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Payments.EventHandlers
{
    public class PaymentSucceededHandler: INotificationHandler<PaymentSucceededEvent>
    {
        private readonly ILogger<PaymentSucceededHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentSucceededHandler(IUnitOfWork unitOfWork,ILogger<PaymentSucceededHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(notification.PaymentId, cancellationToken)
                ?? throw new InvalidOperationException($"Payment {notification.PaymentId} not found.");
           

            var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdAsync(payment.EnrollmentId, cancellationToken)
                ?? throw new InvalidOperationException($"Enrollment {payment.EnrollmentId} not found.");

            if(enrollment.IsActive)
            {
                _logger.LogInformation("Enrollment {EnrollmentId} is already active for payment {PaymentId}.",
                    payment.EnrollmentId, notification.PaymentId);
                return;
            }
            enrollment.Activate();
            _unitOfWork.EnrollmentRepository.Update(enrollment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment {EnrollmentId} activated for payment {PaymentId}.",
                payment.EnrollmentId, notification.PaymentId);
        }
    }
}
