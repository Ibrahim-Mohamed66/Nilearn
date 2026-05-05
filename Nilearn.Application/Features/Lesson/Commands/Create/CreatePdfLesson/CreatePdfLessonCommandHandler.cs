using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Commands.Create.CreatePdfLesson;

internal sealed class CreatePdfLessonCommandHandler : IRequestHandler<CreatePdfLessonCommand, Result<LessonResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePdfLessonCommandHandler> _logger;
    private readonly IMediaService _mediaService;
    public CreatePdfLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePdfLessonCommandHandler> logger, IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediaService = mediaService;
    }
    public async Task<Result<LessonResponse>> Handle(CreatePdfLessonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Creating PDF Lesson");
        var isOwner = await _unitOfWork.SectionRepository.IsOwner(request.SectionId, request.UserId, cancellationToken);
        if (!isOwner)
        {
            _logger.LogWarning("User {UserId} is not the owner of section {SectionId}", request.UserId, request.SectionId);
            throw new ForbiddenAccessException("You are not authorized to create lessons in this section.");
        }

       

        var pdfUploadResult = await _mediaService.UploadDocumentAsync(request.PdfFile.Content, request.PdfFile.FileName, cancellationToken);
        if (pdfUploadResult == null)
        {
            _logger.LogError("Failed to upload PDF for lesson {LessonTitle}", request.Title);
            throw new BadRequestException("Failed to upload PDF");
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
                Format = pdfUploadResult.Format,
                Bytes = pdfUploadResult.Bytes,
                LessonType = LessonType.PDF,
                CloudinaryPublicId = pdfUploadResult.PublicId,
            };

            if (request.IsPreview)
                lesson.MarkAsPreview();
            await _unitOfWork.LessonRepository.AddAsync(lesson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Successfully created PDF lesson with id {LessonId}", lesson.Id);
            var response = new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                LessonType = lesson.LessonType,
                Order = lesson.Order,
                IsLocked = !lesson.IsPreview,
                SectionId = lesson.SectionId,
            };
            return Result<LessonResponse>.SuccessResponse(response, "lesson created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating PDF lesson for section {SectionId}", request.SectionId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            try
            {
                await _mediaService.DeleteDocumentAsync(pdfUploadResult.PublicId);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx,
                    "Failed to rollback document {PublicId}. Manual cleanup required",
                    pdfUploadResult.PublicId);
            }
            throw;
        }
    }
}
