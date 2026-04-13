using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Application.Features.Course.Queries.GetPaged;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

internal sealed class GetCoursePagedQueryHandler
    : IRequestHandler<GetCoursePagedQuery, Result<PagedResponse<CourseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetCoursePagedQueryHandler> _logger;

    public GetCoursePagedQueryHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<GetCoursePagedQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<CourseDto>>> Handle(
        GetCoursePagedQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching courses: Page {Page}, Size {Size}", request.PageNumber, request.PageSize);

        try
        {
            // Get paged courses from repository
            var pagedCourses = await _unitOfWork.CourseRepository.GetPagedCoursesAsync(
                request.PageNumber, request.PageSize, cancellationToken);

            // Map to DTOs with projection
            var courseDtos = pagedCourses.Items
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ThumbnailUrl = _mediaService.GetImageUrl(c.ThumbnailPublicId),

                    CategoryName = c.Category.Name,
                    InstructorName = c.Instructor.User != null
                        ? $"{c.Instructor.User.FirstName} {c.Instructor.User.LastName}"
                        : string.Empty,
                    Price = c.Price,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

           
            var pagedResult = new PagedResponse<CourseDto>
            {
                Items = courseDtos,
                PageNumber = pagedCourses.PageNumber,
                PageSize = pagedCourses.PageSize,
                TotalCount = pagedCourses.TotalCount
            };

            _logger.LogInformation("Successfully retrieved {Count} courses out of {TotalCount}",
                pagedResult.Items.Count, pagedResult.TotalCount);

            return  Result<PagedResponse<CourseDto>>.SuccessResponse(pagedResult, "Courses retrieved successfully.");
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching courses for page {Page} with size {Size}",
                request.PageNumber, request.PageSize);

            return  Result<PagedResponse<CourseDto>>.FailureResponse(message: "Failed to fetch courses. Please try again later.");
            
        }
    }
}