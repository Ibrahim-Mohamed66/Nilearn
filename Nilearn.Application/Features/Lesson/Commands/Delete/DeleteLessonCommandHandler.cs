using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Interfaces;


namespace Nilearn.Application.Features.Lesson.Commands.Delete
{
    internal sealed class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteLessonCommandHandler> _logger;
        private readonly IMediaJobScheduler _mediaJobScheduler;
        public DeleteLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteLessonCommandHandler> logger, IMediaJobScheduler mediaJobScheduler)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediaJobScheduler = mediaJobScheduler;
        }

        public async Task<Result<string>> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id, cancellationToken);
            if (lesson is null)
            {
                _logger.LogWarning("Lesson {LessonId} not found.", request.Id);
                return Result<string>.FailureResponse(message: "Lesson not found.");
            }
            var section = await _unitOfWork.SectionRepository.GetByIdAsync(lesson.SectionId, cancellationToken);
            if (section is null)
            {
                _logger.LogWarning("Section {SectionId} not found for lesson {LessonId}.", lesson.SectionId, request.Id);
                return Result<string>.FailureResponse(message: "Section not found for this lesson.");
            }

            var isOwner = await _unitOfWork.CourseRepository.IsOwner(section.CourseId, request.UserId,cancellationToken);
            if(!isOwner)
            {
                _logger.LogWarning("User {UserId} attempted to delete lesson {LessonId} in course {CourseId} without ownership.", request.UserId, request.Id, section.CourseId);
                return Result<string>.FailureResponse(message: "You do not have permission to delete this lesson.");
            }
            
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                
                var deleted = await _unitOfWork.LessonRepository.DeleteAsync(lesson.Id, cancellationToken);
                if (!deleted)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    _logger.LogWarning("Failed to delete lesson {LessonId} in course {CourseId}.", request.Id, section.CourseId);
                   
                    return Result<string>.FailureResponse(message: "Failed to delete lesson.");
                }
                await _unitOfWork.LessonRepository.DecrementOrderFromAsync(lesson.SectionId, lesson.Order + 1, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                switch (lesson.LessonType)
                {
                    case Domain.Enums.LessonType.Video:

                        await _mediaJobScheduler.EnqueueDeleteVideoAsync(lesson.CloudinaryPublicId);
                        _logger.LogInformation("Deleted video for lesson {LessonId} with Cloudinary public ID {PublicId}.", lesson.Id, lesson.CloudinaryPublicId);
                        break;
                    case Domain.Enums.LessonType.PDF:
                        await _mediaJobScheduler.EnqueueDeleteDocumentAsync(lesson.CloudinaryPublicId);
                        _logger.LogInformation("Deleted document for lesson {LessonId} with Cloudinary public ID {PublicId}.", lesson.Id, lesson.CloudinaryPublicId);
                        break;
                    case Domain.Enums.LessonType.Other:
                        break;
                    default:
                        break;
                }
                _logger.LogInformation("Lesson {LessonId} deleted by user {UserId} in course {CourseId}.", request.Id, request.UserId, section.CourseId);
                return Result<string>.SuccessResponse(message: "Lesson deleted successfully.");
            }

            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                
                _logger.LogError(ex, "An error occurred while deleting lesson {LessonId} by user {UserId} in course {CourseId}.", request.Id, request.UserId, section.CourseId);
                throw;
            }

           
        }
    }
}
