using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdateVideoLesson
{
    internal sealed class UpdateVideoLessonCommandHandler : IRequestHandler<UpdateVideoLessonCommand, Result<LessonResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoLessonCommandHandler> _logger;
        private readonly IMediaService _mediaService;

        public UpdateVideoLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateVideoLessonCommandHandler> logger, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task<Result<LessonResponse>> Handle(UpdateVideoLessonCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id, cancellationToken);
            if (lesson is null)
            {
                _logger.LogWarning("Lesson with id {LessonId} not found", request.Id);
                return Result<LessonResponse>.FailureResponse(message: "Lesson not found");
            }
            else if (lesson.LessonType != Domain.Enums.LessonType.Video)
            {
                _logger.LogWarning("Lesson with id {LessonId} is not a video lesson", request.Id);
                return Result<LessonResponse>.FailureResponse(message: "Invalid lesson type");
            }
            if (lesson.SectionId != request.SectionId)
            {
                _logger.LogWarning("Lesson with id {LessonId} does not belong to section with id {SectionId}", lesson.Id, request.SectionId);
                return Result<LessonResponse>.FailureResponse(message: "Lesson does not belong to the specified section");
            }

            var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.SectionId, cancellationToken);
            if (section is null)
            {
                _logger.LogWarning("Section with id {SectionId} not found", request.SectionId);
                return Result<LessonResponse>.FailureResponse(message: "Section not found");
            }
            var isOwner = await _unitOfWork.CourseRepository.IsOwner(section.CourseId, request.UserId, cancellationToken);
            if (!isOwner)
            {
                _logger.LogWarning("User with id {UserId} is not the owner of the course with id {CourseId}", request.UserId, section.CourseId);
                return Result<LessonResponse>.FailureResponse(["Unauthorized Access"], message: "You Can't Update This Lesson");
            }

            var maxOrder = await _unitOfWork.LessonRepository.GetMaxOrderAsync(request.SectionId, cancellationToken);
            var finalOrder = Math.Clamp(request.Order, 1, maxOrder);

            // Check if anything actually changed
            bool fileChanged = request.VideoFile is not null;
            if (!fileChanged &&
                lesson.Order == finalOrder &&
                lesson.Title == request.Title &&
                lesson.Description == request.Description &&
                lesson.IsPreview == request.IsPreview)
            {
                _logger.LogInformation("No changes detected for lesson with id {LessonId}", lesson.Id);
                return Result<LessonResponse>.SuccessResponse(message: "No changes detected");
            }

            // Upload new video if provided (before transaction)
            MediaUploadResult? videoUploadResult = null;
            string? oldPublicId = null;
            if (fileChanged)
            {
                videoUploadResult = await _mediaService.UploadVideoAsync(request.VideoFile!.Content, request.VideoFile.FileName, cancellationToken);
                if (videoUploadResult is null)
                {
                    _logger.LogError("Failed to upload video for lesson {LessonId}", lesson.Id);
                    return Result<LessonResponse>.FailureResponse(["Failed to upload video"], "Failed to update video lesson");
                }
                oldPublicId = lesson.CloudinaryPublicId;
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                lesson.Title = request.Title;
                lesson.Description = request.Description;

                if (fileChanged && videoUploadResult is not null)
                {
                    lesson.CloudinaryPublicId = videoUploadResult.PublicId;
                    lesson.Format = videoUploadResult.Format;
                    lesson.Bytes = videoUploadResult.Bytes;
                    lesson.DurationInSeconds = videoUploadResult.DurationInSeconds ?? 0;
                }

                if (request.IsPreview)
                {
                    lesson.MarkAsPreview();
                }
                else
                {
                    lesson.UnmarkAsPreview();
                }

                if (lesson.Order != finalOrder)
                {
                    if (finalOrder < lesson.Order)
                    {
                        // Moving up → shift others down
                        await _unitOfWork.LessonRepository
                            .IncrementOrderRangeAsync(request.SectionId, finalOrder, lesson.Order - 1, cancellationToken);
                    }
                    else if (finalOrder > lesson.Order)
                    {
                        // Moving down → shift others up
                        await _unitOfWork.LessonRepository
                            .DecrementOrderRangeAsync(request.SectionId, lesson.Order + 1, finalOrder, cancellationToken);
                    }

                    lesson.Order = finalOrder;
                }

                _unitOfWork.LessonRepository.Update(lesson);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // Delete old video from Cloudinary after successful commit
                if (oldPublicId is not null)
                {
                    try
                    {
                        await _mediaService.DeleteVideoAsync(oldPublicId);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx,
                            "Failed to delete old video {PublicId}. Manual cleanup required",
                            oldPublicId);
                    }
                }

                _logger.LogInformation("Lesson with id {LessonId} updated successfully", lesson.Id);
                var lessonResponse = new LessonResponse
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Description = lesson.Description,
                    Order = lesson.Order,
                    SectionId = lesson.SectionId,
                    LessonType = lesson.LessonType
                };
                return Result<LessonResponse>.SuccessResponse(lessonResponse, message: "Lesson updated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // If we uploaded a new video, roll it back
                if (videoUploadResult is not null)
                {
                    try
                    {
                        await _mediaService.DeleteVideoAsync(videoUploadResult.PublicId);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx,
                            "Failed to rollback video {PublicId}. Manual cleanup required",
                            videoUploadResult.PublicId);
                    }
                }

                _logger.LogError(ex, "Error updating lesson with id {LessonId}", lesson.Id);
                throw;
            }
        }
    }
}
