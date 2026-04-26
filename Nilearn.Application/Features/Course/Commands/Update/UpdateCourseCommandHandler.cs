using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Course.Commands.Update;

internal sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;
    private readonly IMediaJobScheduler _mediaJobScheduler;

    public UpdateCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<UpdateCourseCommandHandler> logger,
        IMediaJobScheduler mediaJobScheduler
             )
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
        _mediaJobScheduler = mediaJobScheduler;
    }

    public async Task<Result<string>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting course update: {CourseId}", request.CourseId);

        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course == null)
        {
            _logger.LogWarning("Course not found for Id: {CourseId}", request.CourseId);
            return Result<string>.FailureResponse(message: "Course not found.");
        }

        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        
        if (instructorId == null || course.InstructorId != instructorId.Value)
        {
            _logger.LogWarning("Unauthorized course update attempt by UserId: {UserId} for CourseId: {CourseId}", request.UserId, request.CourseId);
            return Result<string>.FailureResponse(message: "Unauthorized to update this course.");
        }
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Invalid CategoryId: {CategoryId}", request.CategoryId);
            return Result<string>.FailureResponse(message: "Invalid category.");
        }


        string newThumbnailPublicId = null;
        string? oldThumbnailPublicId = course.ThumbnailPublicId;

        if (request.Thumbnail != null)
        {
            try
            {
                var result = await _mediaService.UploadImageAsync(request.Thumbnail.Content, request.Thumbnail.FileName,request.Purpose,cancellationToken);
                newThumbnailPublicId = result.PublicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload new thumbnail for course: {CourseId}", request.CourseId);
                return Result<string>.FailureResponse(message: "Failed to upload thumbnail.");
            }
        }
        

        try
        {
            course.Title = request.Title;
            course.Description = request.Description;
            course.CategoryId = request.CategoryId;
            course.Price = request.Price;
            course.Slug = GenerateSlug(request.Title);
            
            if (newThumbnailPublicId != null)
            {
                course.ThumbnailPublicId = newThumbnailPublicId;
            }
            if(request.IsPublished)
            {
                course.Publish();
            }
            else
            {
                course.Unpublish();
            }

            _unitOfWork.CourseRepository.Update(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course updated successfully with Id: {CourseId}", course.Id);

            if (newThumbnailPublicId != null && !string.IsNullOrEmpty(oldThumbnailPublicId))
            {
                try
                {
                    await _mediaJobScheduler.EnqueueDeleteImageAsync(oldThumbnailPublicId);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to delete old thumbnail: {PublicId}", oldThumbnailPublicId);
                }
            }

            return Result<string>.SuccessResponse(message: "Course updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error while updating course: {CourseId}", request.CourseId);

            if (newThumbnailPublicId != null)
            {
                try
                {
                    await _mediaJobScheduler.EnqueueDeleteImageAsync(newThumbnailPublicId);
                    _logger.LogWarning("Rolled back uploaded thumbnail: {PublicId}", newThumbnailPublicId);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to rollback thumbnail: {PublicId}", newThumbnailPublicId);
                }
            }

            return Result<string>.FailureResponse(message: "Database error while updating the course.");
        }
    }

    private string GenerateSlug(string title)
    {
        return title.ToLower().Replace(" ", "-").Replace(".", "").Replace(",", "");
    }
}
