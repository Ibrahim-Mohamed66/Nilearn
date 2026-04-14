using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Course.Commands.Create;

internal sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<CreateCourseResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<CreateCourseResponse>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting course creation: {Title}", request.Title);

        string thumbnailPublicId;

        try
        {
            thumbnailPublicId = await _mediaService.UploadImageAsync(
                request.Thumbnail.Content,
                request.Thumbnail.FileName,
                request.Purpose
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload thumbnail for course: {Title}", request.Title);
            throw; 
        }
        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        if(instructorId == null )
        {
            _logger.LogError("Instructor not found for UserId: {UserId}", request.UserId);
            throw new InvalidOperationException("Instructor not found for the given UserId.");
        }
        var slug = GenerateSlug(request.Title);

        var course = new Domain.Entities.Course
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            ThumbnailPublicId = thumbnailPublicId,
            Price = request.Price,
            Slug = slug,
            InstructorId = instructorId.Value

        };

        if(request.IsPublished)
        {
            course.Publish();
        }

        try
        {
            await _unitOfWork.CourseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course created successfully with Id: {CourseId}", course.Id);

            var courseResponse = new CreateCourseResponse(course.Id);
            
            return Result<CreateCourseResponse>.SuccessResponse(courseResponse, message:"Course created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error while creating course: {Title}", request.Title);

            try
            {
                await _mediaService.DeleteImageAsync(thumbnailPublicId);
                _logger.LogWarning("Rolled back uploaded thumbnail: {PublicId}", thumbnailPublicId);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Failed to rollback thumbnail: {PublicId}", thumbnailPublicId);
            }

            throw;
        }
    }



    private string GenerateSlug(string title)
    {
        return title.ToLower().Replace(" ", "-").Replace(".", "").Replace(",", "");
    }
}