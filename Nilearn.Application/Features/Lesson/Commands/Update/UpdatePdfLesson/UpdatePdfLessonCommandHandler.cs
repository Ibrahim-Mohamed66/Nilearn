using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdatePdfLesson
{
    internal sealed class UpdatePdfLessonCommandHandler : IRequestHandler<UpdatePdfLessonCommand, Result<LessonResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePdfLessonCommandHandler> _logger;
        private readonly IMediaService _mediaService;

        public UpdatePdfLessonCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePdfLessonCommandHandler> logger, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task<Result<LessonResponse>> Handle(UpdatePdfLessonCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id, cancellationToken);
            if (lesson is null)
            {
                _logger.LogWarning("Lesson with id {LessonId} not found", request.Id);
                return Result<LessonResponse>.FailureResponse(message: "Lesson not found");
            }
            else if (lesson.LessonType != Domain.Enums.LessonType.PDF)
            {
                _logger.LogWarning("Lesson with id {LessonId} is not a PDF lesson", request.Id);
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
            bool fileChanged = request.PdfFile is not null;
            if (!fileChanged &&
                lesson.Order == finalOrder &&
                lesson.Title == request.Title &&
                lesson.Description == request.Description &&
                lesson.IsPreview == request.IsPreview)
            {
                _logger.LogInformation("No changes detected for lesson with id {LessonId}", lesson.Id);
                return Result<LessonResponse>.SuccessResponse(message: "No changes detected");
            }

            // Upload new PDF if provided (before transaction)
            MediaUploadResult? pdfUploadResult = null;
            string? oldPublicId = null;
            if (fileChanged)
            {
                pdfUploadResult = await _mediaService.UploadDocumentAsync(request.PdfFile!.Content, request.PdfFile.FileName, cancellationToken);
                if (pdfUploadResult is null)
                {
                    _logger.LogError("Failed to upload PDF for lesson {LessonId}", lesson.Id);
                    return Result<LessonResponse>.FailureResponse(["Failed to upload PDF"], "Failed to update PDF lesson");
                }
                oldPublicId = lesson.CloudinaryPublicId;
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                lesson.Title = request.Title;
                lesson.Description = request.Description;

                if (fileChanged && pdfUploadResult is not null)
                {
                    lesson.CloudinaryPublicId = pdfUploadResult.PublicId;
                    lesson.Format = pdfUploadResult.Format;
                    lesson.Bytes = pdfUploadResult.Bytes;
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

                // Delete old PDF from Cloudinary after successful commit
                if (oldPublicId is not null)
                {
                    try
                    {
                        await _mediaService.DeleteDocumentAsync(oldPublicId);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx,
                            "Failed to delete old document {PublicId}. Manual cleanup required",
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

                // If we uploaded a new PDF, roll it back
                if (pdfUploadResult is not null)
                {
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
                }

                _logger.LogError(ex, "Error updating lesson with id {LessonId}", lesson.Id);
                throw;
            }
        }
    }
}
