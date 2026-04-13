using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Application.Features.Course.Queries.GetById;
using Nilearn.Domain.Interfaces;

internal sealed class GetCourseByIdQueryHandler
    : IRequestHandler<GetCourseByIdQuery, Result<CourseDto?>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetCourseByIdQueryHandler> _logger;

    public GetCourseByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<GetCourseByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<CourseDto?>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching course with ID: {CourseId}", request.Id);

        var course = await _unitOfWork.CourseRepository
            .GetCourseByIdWithDetailsAsync(request.Id, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course with ID: {CourseId} was not found.", request.Id);
            return Result<CourseDto>.FailureResponse(message:"Course is Not Found" );
        }
        var dto = new CourseDto
        {
            Id = course.Id,
            CategoryName = course.Category.Name,
            InstructorName = $"{course.Instructor.User.FirstName} {course.Instructor.User.LastName}",
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt,
            ThumbnailUrl = _mediaService.GetImageUrl(course.ThumbnailPublicId),
            Price = course.Price,
            Title = course.Title,
            
            
            
        };

        _logger.LogInformation(
                    "Course with ID: {CourseId} retrieved successfully. Title: {CourseTitle}",
                    course.Id,
                    course.Title);

        return Result<CourseDto>.SuccessResponse(dto, "Course retrieved successfully.");
    }
}