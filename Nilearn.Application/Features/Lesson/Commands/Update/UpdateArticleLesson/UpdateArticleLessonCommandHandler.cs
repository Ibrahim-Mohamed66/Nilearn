using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdateArticleLesson
{
    internal sealed class UpdateArticleLessonCommandHandler : IRequestHandler<UpdateArticleLessonCommand, Result<LessonResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateArticleLessonCommandHandler> _logger;

        public UpdateArticleLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateArticleLessonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<LessonResponse>> Handle(UpdateArticleLessonCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id,cancellationToken);
            if(lesson is  null) 
            {
               _logger.LogWarning("Lesson with id {LessonId} not found", request.Id);
                throw new NotFoundException("Lesson", request.Id);
            }
            else if(lesson.LessonType != Domain.Enums.LessonType.Article)
            {
                _logger.LogWarning("Lesson with id {LessonId} is not an article lesson", request.Id);
                throw new BadRequestException("Invalid lesson type. Expected Article.");
            }
            if(lesson.SectionId != request.sectionId)
            {
                _logger.LogWarning("Lesson with id {LessonId} does not belong to section with id {SectionId}", lesson.Id, request.sectionId);
                throw new BadRequestException("Lesson does not belong to the specified section.");
            }

            var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.sectionId, cancellationToken);
            if(section is null)
            {
                _logger.LogWarning("Section with id {SectionId} not found", request.sectionId);
                throw new NotFoundException("Section", request.sectionId);
            }
            var isOwner = await _unitOfWork.CourseRepository.IsOwner(section.CourseId, request.UserId, cancellationToken);
            if (!isOwner)
            {
                _logger.LogWarning("User with id {UserId} is not the owner of the course with id {CourseId}", request.UserId, section.CourseId);
                throw new ForbiddenAccessException("You are not authorized to update this lesson.");
            }
            var maxOrder = await _unitOfWork.LessonRepository.GetMaxOrderAsync(request.sectionId, cancellationToken);

            var finalOrder = Math.Clamp(request.Order, 1, maxOrder);

            if (lesson.Order == finalOrder &&
                 lesson.Title == request.Title &&
                 lesson.Description == request.Description &&
                 lesson.Content == request.Content &&
                 lesson.IsPreview == request.IsPreview)
            {
               _logger.LogInformation("No changes detected for lesson with id {LessonId}", lesson.Id);
                return Result<LessonResponse>.SuccessResponse(message: "No changes detected");
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                lesson.Title = request.Title;
                lesson.Description = request.Description;
                lesson.Content = request.Content;
                if(request.IsPreview)
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
                            .IncrementOrderRangeAsync(request.sectionId, finalOrder, lesson.Order - 1, cancellationToken);
                    }
                    else if (finalOrder > lesson.Order)
                    {
                        // Moving down → shift others up
                        await _unitOfWork.LessonRepository
                            .DecrementOrderRangeAsync(request.sectionId, lesson.Order + 1, finalOrder, cancellationToken);
                    }

                    lesson.Order = finalOrder;
                }

                _unitOfWork.LessonRepository.Update(lesson);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
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
                _logger.LogError(ex, "Error updating lesson with id {LessonId}", lesson.Id);
                throw;
            }
        }
    }
}
