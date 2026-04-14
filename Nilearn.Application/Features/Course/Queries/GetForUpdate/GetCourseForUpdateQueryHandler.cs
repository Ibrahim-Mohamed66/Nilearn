using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Course.Queries.GetForUpdate;

internal sealed class GetCourseForUpdateQueryHandler
    : IRequestHandler<GetCourseForUpdateQuery, Result<CourseForUpdateDto?>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetCourseForUpdateQueryHandler> _logger;

    public GetCourseForUpdateQueryHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<GetCourseForUpdateQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<CourseForUpdateDto?>> Handle(GetCourseForUpdateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching course for update with ID: {CourseId}", request.Id);

        var course = await _unitOfWork.CourseRepository
            .GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course with ID: {CourseId} was not found.", request.Id);
            return Result<CourseForUpdateDto>.FailureResponse(message: "Course is Not Found");
        }

        var dto = new CourseForUpdateDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Price = course.Price,
            ThumbnailUrl = _mediaService.GetImageUrl(course.ThumbnailPublicId)
        };

        _logger.LogInformation(
                    "Course for update with ID: {CourseId} retrieved successfully. Title: {CourseTitle}",
                    course.Id,
                    course.Title);

        return Result<CourseForUpdateDto>.SuccessResponse(dto, "Course for update retrieved successfully.");
    }
}
