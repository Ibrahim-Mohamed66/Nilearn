using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;


namespace Nilearn.Application.Features.Lesson.Commands.Create.CreateVideoLesson
{
    public sealed class CreateVideoLessonCommandHandler : IRequestHandler<CreateVideoLessonCommand, Result<LessonResponse>>
    {
        private readonly ILogger<CreateVideoLessonCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;
        public CreateVideoLessonCommandHandler(ILogger<CreateVideoLessonCommandHandler> logger, IUnitOfWork unitOfWork, IMediaService mediaService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
        }

        public async Task<Result<LessonResponse>> Handle(CreateVideoLessonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Creating Video Lessons ");

            var section = await _unitOfWork.SectionRepository
              .GetByIdAsync(request.SectionId, cancellationToken);
            if (section is null)
            {
                _logger.LogWarning("Section {SectionId} not found", request.SectionId);
                throw new NotFoundException("Section", request.SectionId);
            }
            var isOwner = await _unitOfWork.CourseRepository.IsOwner(section.CourseId, request.UserId, cancellationToken);
            if (!isOwner)
            {
                _logger.LogWarning("User {UserId} is not the owner of section {SectionId}", request.UserId, request.SectionId);
                throw new ForbiddenAccessException("You are not authorized to create lessons in this section.");
            }

            var videoUploadResult = await _mediaService.UploadVideoAsync(request.VideoFile.Content, request.VideoFile.FileName, cancellationToken);
            if (videoUploadResult == null)
            {
                _logger.LogError("Failed to upload video for lesson {LessonTitle}", request.Title);
                throw new BadRequestException("Failed to upload video");
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var maxOrder = await _unitOfWork.LessonRepository.GetMaxOrderAsync(request.SectionId, cancellationToken);

                var finalOrder = request.Order > maxOrder + 1
                            ? maxOrder + 1
                            : request.Order;
                if (finalOrder <= maxOrder)
                    await _unitOfWork.LessonRepository
                            .IncrementOrderFromAsync(request.SectionId, finalOrder, cancellationToken);

                var lesson = new Domain.Entities.Lesson
                {
                    Title = request.Title,
                    Description = request.Description,
                    SectionId = request.SectionId,
                    Order = finalOrder,
                    DurationInSeconds = videoUploadResult.DurationInSeconds ?? 0,
                    Format = videoUploadResult.Format,
                    Bytes = videoUploadResult.Bytes,
                    LessonType = LessonType.Video,
                    CloudinaryPublicId = videoUploadResult.PublicId,
                };

                if(request.IsPreview)
                    lesson.MarkAsPreview();

                await _unitOfWork.LessonRepository.AddAsync(lesson, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully created video lesson with id {LessonId}", lesson.Id);

                var response = new LessonResponse
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Description = lesson.Description,
                    LessonType = lesson.LessonType,
                    Order = lesson.Order,
                    SectionId = lesson.SectionId,
                    
                };

                return Result<LessonResponse>.SuccessResponse(response, "lesson created successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating video lesson. Rolling back transaction.");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
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
                throw;
            }

        }
    }
}
