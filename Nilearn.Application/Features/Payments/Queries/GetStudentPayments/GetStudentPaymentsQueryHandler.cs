using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Payments.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Payments.Queries.GetStudentPayments;

internal sealed class GetStudentPaymentsQueryHandler
    : IRequestHandler<GetStudentPaymentsQuery, Result<PagedResponse<PaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetStudentPaymentsQueryHandler> _logger;

    public GetStudentPaymentsQueryHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<GetStudentPaymentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<PaymentDto>>> Handle(
        GetStudentPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching payments for student (UserId: {UserId}): Page {Page}, Size {Size}",
            request.UserId, request.PageNumber, request.PageSize);

        var student = await _unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
        if (student is null)
        {
            _logger.LogWarning("Student not found for UserId: {UserId}", request.UserId);
            throw new NotFoundException("Student", request.UserId);
        }

        // Query payments through the enrollment relationship
        var query = _unitOfWork.PaymentRepository.QueryPaymentHistory(student.Id, request.Status);

        // Project to DTO before pagination
        var projectedQuery = query.Select(p => new PaymentDto
        {
            PaymentId = p.Id,
            EnrollmentId = p.EnrollmentId,
            Amount = p.Amount,
            Currency = p.Currency,
            Status = p.Status,
            PaidAt = p.PaidAt,
            TransactionId = p.PaymobTransactionId,
            CourseTitle = p.Enrollment.Course!.Title,
            CourseThumbnailUrl = p.Enrollment.Course.ThumbnailPublicId
        });

        // Paginate the projected query
        var pagedPayments = await projectedQuery.ToPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        // Resolve image URLs in memory
        foreach (var item in pagedPayments.Items)
        {
            if (!string.IsNullOrEmpty(item.CourseThumbnailUrl))
            {
                item.CourseThumbnailUrl = _mediaService.GetImageUrl(item.CourseThumbnailUrl);
            }
        }

        _logger.LogInformation("Successfully retrieved {Count} payments for student {StudentId} out of {TotalCount}",
            pagedPayments.Items.Count, student.Id, pagedPayments.TotalCount);

        return Result<PagedResponse<PaymentDto>>.SuccessResponse(pagedPayments, "Payments retrieved successfully");
    }
}
