using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Enrollment.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Enrollment.Queries.GetStudentEnrollments;

internal sealed class GetStudentEnrollmentsQueryHandler
    : IRequestHandler<GetStudentEnrollmentsQuery, Result<PagedResponse<EnrollmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetStudentEnrollmentsQueryHandler> _logger;

    public GetStudentEnrollmentsQueryHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<GetStudentEnrollmentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<EnrollmentDto>>> Handle(
    GetStudentEnrollmentsQuery request,
    CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching enrollments for student (UserId: {UserId}): Page {Page}, Size {Size}",
            request.UserId, request.PageNumber, request.PageSize);

        try
        {
            var student = await _unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
            if (student is null)
            {
                _logger.LogWarning("Student not found for UserId: {UserId}", request.UserId);
                return Result<PagedResponse<EnrollmentDto>>.FailureResponse(message: "Student not found");
            }

            // 1. Get the IQueryable from the repo
            var query = _unitOfWork.EnrollmentRepository.QueryByStudentId(student.Id, request.Status);

            // 2. Map to DTO *BEFORE* pagination. 
            // Notice there is NO .ToList() at the end of this!
            var projectedQuery = query.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                Status = e.Status,
                ActivatedAt = e.ActivatedAt,
                CreatedAt = e.CreatedAt,
                CourseTitle = e.Course.Title,
                CourseThumbnailUrl = e.Course.ThumbnailPublicId,
                InstructorName = (e.Course.Instructor.User.FirstName ?? "") + " " + (e.Course.Instructor.User.LastName ?? "")
            });

            // 3. Paginate the projected query. EF Core turns step 2 into SQL here.
            var pagedEnrollments = await projectedQuery.ToPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            // 4. Resolve the image URLs in memory
            foreach (var item in pagedEnrollments.Items)
            {
                if (!string.IsNullOrEmpty(item.CourseThumbnailUrl))
                {
                    item.CourseThumbnailUrl = _mediaService.GetImageUrl(item.CourseThumbnailUrl);
                }
            }

            _logger.LogInformation("Successfully retrieved {Count} enrollments for student {StudentId} out of {TotalCount}",
                pagedEnrollments.Items.Count, student.Id, pagedEnrollments.TotalCount);

            return Result<PagedResponse<EnrollmentDto>>.SuccessResponse(pagedEnrollments, "Enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enrollments for student (UserId: {UserId})", request.UserId);
            return Result<PagedResponse<EnrollmentDto>>.FailureResponse(message: "Failed to fetch enrollments. Please try again later.");
        }
    }
}
