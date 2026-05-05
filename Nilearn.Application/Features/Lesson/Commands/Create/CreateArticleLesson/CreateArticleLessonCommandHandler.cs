using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Commands.Create.CreateArticleLesson;

internal sealed class CreateArticleLessonCommandHandler : IRequestHandler<CreateArticleLessonCommand, Result<LessonResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateArticleLessonCommandHandler> _logger;

    public CreateArticleLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateArticleLessonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LessonResponse>> Handle(CreateArticleLessonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Creating Article Lesson");
        var isOwner = await _unitOfWork.SectionRepository.IsOwner(request.SectionId, request.UserId, cancellationToken);
        if (!isOwner)
        {
            _logger.LogWarning("User {UserId} is not the owner of section {SectionId}", request.UserId, request.SectionId);
            throw new ForbiddenAccessException("You are not authorized to create lessons in this section.");
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
                Content = request.Content,
                LessonType = LessonType.Article,
            };

            if (request.IsPreview)
                lesson.MarkAsPreview();

            await _unitOfWork.LessonRepository.AddAsync(lesson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Successfully created Article lesson with id {LessonId}", lesson.Id);
            
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
            _logger.LogError(ex, "An error occurred while creating Article lesson for section {SectionId}", request.SectionId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
