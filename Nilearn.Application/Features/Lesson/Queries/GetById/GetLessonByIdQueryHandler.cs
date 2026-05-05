using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Queries.GetById;

internal sealed class GetLessonByIdQueryHandler : IRequestHandler<GetLessonByIdQuery, Result<LessonResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetLessonByIdQueryHandler> _logger;
    private readonly IMediaService _mediaService;

    public GetLessonByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetLessonByIdQueryHandler> logger,
        IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediaService = mediaService;
    }

    public async Task<Result<LessonResponse>> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching lesson with ID: {LessonId}", request.Id);
        
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lesson is null)
        {
            _logger.LogWarning("Lesson with ID: {LessonId} was not found.", request.Id);
            throw new NotFoundException("Lesson", request.Id);
        }
               
        var hasAccess = lesson.IsPreview;

        if (!hasAccess && !string.IsNullOrEmpty(request.UserId))
        {
            // Check if user is the instructor of the course
            hasAccess = await _unitOfWork.SectionRepository.IsOwner(lesson.SectionId, request.UserId, cancellationToken);

            if (!hasAccess)
            {
                // Check if user is enrolled
                var student = await _unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
                if (student is not null)
                {
                    var section = await _unitOfWork.SectionRepository.GetByIdAsync(lesson.SectionId, cancellationToken);
                    if (section is not null)
                    {
                        hasAccess = await _unitOfWork.EnrollmentRepository.IsEnrolledAsync(student.Id, section.CourseId, cancellationToken);
                    }
                }
            }
        }

        var response = MapToResponse(lesson, hasAccess);

        _logger.LogInformation("Successfully fetched lesson with ID: {LessonId}", request.Id);
        return Result<LessonResponse>.SuccessResponse(response, "Lesson fetched successfully.");
    }

    private LessonResponse MapToResponse(Domain.Entities.Lesson lesson, bool hasAccess)
    {
        return lesson.LessonType switch
        {
            LessonType.Video => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                IsLocked = !hasAccess,
                VideoUrl = hasAccess && !string.IsNullOrEmpty(lesson.CloudinaryPublicId) ? _mediaService.GetVideoUrl(lesson.CloudinaryPublicId) : null,
                PdfUrl = null,
                ArticleContent = null
            },
            LessonType.PDF => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                IsLocked = !hasAccess,
                PdfUrl = hasAccess && !string.IsNullOrEmpty(lesson.CloudinaryPublicId) ? _mediaService.GetDocumentUrl(lesson.CloudinaryPublicId) : null,
                VideoUrl = null,
                ArticleContent = null
            },
            LessonType.Article => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                IsLocked = !hasAccess,
                ArticleContent = hasAccess ? lesson.Content : null,
                VideoUrl = null,
                PdfUrl = null
            },
            _ => throw new InvalidOperationException($"Unsupported lesson type: {lesson.LessonType}")
        };
    }
}
