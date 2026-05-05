using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Enrollment.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Enrollment.Queries.GetCourseEnrollments;

internal sealed class GetCourseEnrollmentsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCourseEnrollmentsQueryHandler> logger,
    IMediaService mediaService) 
    : IRequestHandler<GetCourseEnrollmentsQuery, Result<PagedResponse<EnrollmentDto>>>
{
    public async Task<Result<PagedResponse<EnrollmentDto>>> Handle(
        GetCourseEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching enrollments for course {CourseId}: Page {Page}, Size {Size}",
            request.CourseId, request.PageNumber, request.PageSize);

        var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            logger.LogWarning("Course not found: {CourseId}", request.CourseId);
            throw new NotFoundException("Course", request.CourseId);
        }

            var query = unitOfWork.EnrollmentRepository.QueryByCourseId(request.CourseId, request.Status);

            var projectedQuery = query.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                Status = e.Status,
                ActivatedAt = e.ActivatedAt,
                CreatedAt = e.CreatedAt,
                StudentName = (e.Student.AppUser.FirstName ?? "") + " " + (e.Student.AppUser.LastName ?? ""),
                StudentEmail = e.Student.AppUser.Email,               
                CourseThumbnailUrl = e.Course.ThumbnailPublicId,
                CourseTitle = e.Course.Title
            });

            var pagedEnrollments = await projectedQuery.ToPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            
            foreach (var item in pagedEnrollments.Items)
            {
                if (!string.IsNullOrEmpty(item.CourseThumbnailUrl))
                {
                    item.CourseThumbnailUrl = mediaService.GetImageUrl(item.CourseThumbnailUrl);
                }
            }

            logger.LogInformation("Successfully retrieved {Count} enrollments for course {CourseId} out of {TotalCount}",
                pagedEnrollments.Items.Count, request.CourseId, pagedEnrollments.TotalCount);

        return Result<PagedResponse<EnrollmentDto>>.SuccessResponse(pagedEnrollments, "Enrollments retrieved successfully");
    }
}