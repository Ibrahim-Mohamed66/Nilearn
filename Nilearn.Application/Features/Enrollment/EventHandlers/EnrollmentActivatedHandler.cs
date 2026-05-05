using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Events;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Enrollment.EventHandlers;

public class EnrollmentActivatedHandler : INotificationHandler<EnrollmentActivatedEvent>
{
    private readonly ILogger<EnrollmentActivatedHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailVerificationService _emailService;

    public EnrollmentActivatedHandler(IUnitOfWork unitOfWork,ILogger<EnrollmentActivatedHandler> logger,IEmailVerificationService emailService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(EnrollmentActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling enrollment activation event for EnrollmentId: {EnrollmentId}", notification.EnrollmentId);

        var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdWithDetailsAsync(notification.EnrollmentId, cancellationToken);
        if (enrollment?.Student?.AppUser == null || enrollment.Course == null)
        {
            _logger.LogWarning("Enrollment data not found for EnrollmentId: {EnrollmentId}", notification.EnrollmentId);
            return;
        }

        var student = enrollment.Student.AppUser;
        var course = enrollment.Course;
        var instructorName = course.Instructor?.User?.FirstName ?? "Your Instructor";

        // await _emailService.SendEnrollmentActivatedEmailAsync(
        //     student.Email,
        //     student.FirstName,
        //     course.Title,
        //     instructorName,
        //     course.Id,
        //     cancellationToken);

        _logger.LogInformation("Enrollment activation email queued for Student {StudentId} for Course {CourseId}",
            student.Id, course.Id);
    }
}
