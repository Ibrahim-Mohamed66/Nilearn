using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Enrollment.Commands.Create.DTOs;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Services;

internal class EnrollmentService : IEnrollmentService
{
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;

    public EnrollmentService(ILogger<EnrollmentService> logger, IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result<CreateEnrollmentResponse>> CreateEnrollmentAsync(string userId, int courseId, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.StudentRepository
            .GetByUserId(userId, cancellationToken);

        if (student is null)
        {
            _logger.LogWarning("Enrollment failed: Student not found for UserId {UserId}", userId);
            throw new NotFoundException("Student", userId);
        }

        var course = await _unitOfWork.CourseRepository
            .GetByIdAsync(courseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Enrollment failed: Course not found for CourseId {CourseId}", courseId);
            throw new NotFoundException("Course", courseId);
        }

        var enrollment = await _unitOfWork.EnrollmentRepository
            .GetByStudentAndCourseAsync(student.Id, course.Id, cancellationToken);
            
        var isFree = course.Price == 0;
        Payment? payment = null;

        try
        {
            if (enrollment is not null)
            {
                var existingResult = await HandleExistingEnrollmentAsync(enrollment, student, course, isFree, cancellationToken);
                if (existingResult != null) return existingResult;
            }
            else
            {
                var newEnrollmentResult = await HandleNewEnrollmentAsync(student, course, isFree, cancellationToken);
                enrollment = newEnrollmentResult.Enrollment;
                if (newEnrollmentResult.Result != null) return newEnrollmentResult.Result;
            }

            payment = await CreatePaymentRecordAsync(enrollment.Id, course.Price, cancellationToken);
            var paymentUrl = await ProcessPaymentGatewayAsync(payment, enrollment, student, course, cancellationToken);

            return Result<CreateEnrollmentResponse>.SuccessResponse(
                new CreateEnrollmentResponse(true, paymentUrl),
                "Enrollment created successfully");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database constraint violation for UserId {UserId}", userId);
            throw new ConflictException("Enrollment", "Student already enrolled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Paymob failed for UserId {UserId}. Rolling back DB records.", userId);
            await RollbackFailedEnrollmentAsync(payment, enrollment, isFree, cancellationToken);
            throw new BadRequestException("Payment gateway unavailable. Please try again.");
        }
    }

    private async Task<Result<CreateEnrollmentResponse>?> HandleExistingEnrollmentAsync(
        Domain.Entities.Enrollment enrollment, Student student, Course course, bool isFree, CancellationToken cancellationToken)
    {
        if (enrollment.IsCancelled)
        {
            _logger.LogInformation("Reactivating cancelled enrollment for Student {StudentId} and Course {CourseId}", student.Id, course.Id);
            if (isFree)
            {
                enrollment.Activate();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<CreateEnrollmentResponse>.SuccessResponse(
                    new CreateEnrollmentResponse(RequiresPayment: false, PaymentUrl: null),
                    "Student re-enrolled successfully");
            }
            else
            {
                enrollment.Reactivate();
                _unitOfWork.EnrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Enrollment reactivated but payment required for Student {StudentId} and Course {CourseId}", student.Id, course.Id);
                return null;
            }
        }
        else if (enrollment.IsActive)
        {
            _logger.LogInformation("Student {StudentId} already enrolled in Course {CourseId}", student.Id, course.Id);
            throw new ConflictException("Enrollment", "Student already enrolled in this course.");
        }
        
        var existingPayments = await _unitOfWork.PaymentRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
        if (existingPayments.Any(p => p.IsPending))
        {
            _logger.LogInformation("Student {StudentId} already has a pending payment for Course {CourseId}", student.Id, course.Id);
            throw new ConflictException("Payment", "A payment is already pending for this course.");
        }

        return null;
    }

    private async Task<(Domain.Entities.Enrollment Enrollment, Result<CreateEnrollmentResponse>? Result)> HandleNewEnrollmentAsync(
        Student student, Course course, bool isFree, CancellationToken cancellationToken)
    {
        var enrollment = new Domain.Entities.Enrollment(student.Id, course.Id);

        if (isFree)
        {
            enrollment.Activate();
            await _unitOfWork.EnrollmentRepository.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Free enrollment completed successfully. EnrollmentId: {EnrollmentId}", enrollment.Id);

            return (enrollment, Result<CreateEnrollmentResponse>.SuccessResponse(
                new CreateEnrollmentResponse(RequiresPayment: false, PaymentUrl: null),
                "Student enrolled successfully"));
        }

        await _unitOfWork.EnrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (enrollment, null);
    }

    private async Task<Payment> CreatePaymentRecordAsync(int enrollmentId, decimal price, CancellationToken cancellationToken)
    {
        var payment = new Payment(enrollmentId, price);
        await _unitOfWork.PaymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return payment;
    }

    private async Task<string> ProcessPaymentGatewayAsync(
        Payment payment, Domain.Entities.Enrollment enrollment, Student student, Course course, CancellationToken cancellationToken)
    {
        var paymentSessionRequest = new PaymentSessionRequest
        {
            Amount = payment.Amount,
            CoursePrice = course.Price,
            CourseTitle = course.Title,
            Currency = payment.Currency.ToString(),
            MerchantReferenceId = payment.MerchantReferenceId,
            FirstName = student.AppUser.FirstName,
            LastName = student.AppUser.LastName,
            Email = student.AppUser.Email,
            PhoneNumber = student.AppUser.PhoneNumber,
            EnrollmentId = enrollment.Id,
        };

        var paymentSession = await _paymentGateway
            .CreatePaymentSessionAsync(paymentSessionRequest, cancellationToken);

        payment.SetPaymobData(paymentSession.PaymobIntentionId, paymentSession.PaymobOrderId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enrollment created with payment. EnrollmentId: {EnrollmentId}, PaymentUrl: {PaymentUrl}", enrollment.Id, paymentSession.PaymentUrl);

        return paymentSession.PaymentUrl;
    }

    private async Task RollbackFailedEnrollmentAsync(Payment? payment, Domain.Entities.Enrollment? enrollment, bool isFree, CancellationToken cancellationToken)
    {
        try
        {
            if (payment is not null)
                payment.MarkAsFailed();

            if (enrollment is not null && !isFree)
                enrollment.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully cleaned up orphaned records for EnrollmentId {EnrollmentId}", enrollment?.Id);
        }
        catch (Exception cleanupEx)
        {
            _logger.LogCritical(cleanupEx, "CRITICAL: Failed to clean up orphaned records for EnrollmentId {EnrollmentId}", enrollment?.Id);
        }
    }
}
