using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Lesson.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Lesson.Queries.GetAll;

internal sealed class GetAllLessonsQueryHandler : IRequestHandler<GetAllLessonsQuery, Result<List<LessonResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllLessonsQueryHandler> _logger;
    private readonly IMediaService _mediaService;

    public GetAllLessonsQueryHandler(
        IUnitOfWork unitOfWork, 
        ILogger<GetAllLessonsQueryHandler> logger,
        IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediaService = mediaService;
    }

    public async Task<Result<List<LessonResponse>>> Handle(GetAllLessonsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching lessons for SectionId: {SectionId}", request.SectionId);

        var lessons = await _unitOfWork.LessonRepository.GetAllBySectionIdAsync(request.SectionId, cancellationToken);
        if(!lessons.Any())
        {
            _logger.LogWarning("No lessons found for SectionId: {SectionId}", request.SectionId);
            return Result<List<LessonResponse>>.SuccessResponse(new List<LessonResponse>(), "No lessons found.");
        }

        var response = lessons.Select(MapToResponse).ToList();


        _logger.LogInformation("Successfully fetched {Count} lessons for SectionId: {SectionId}", response.Count, request.SectionId);
        return Result<List<LessonResponse>>.SuccessResponse(response, "Lessons fetched successfully.");
    }

    private LessonResponse MapToResponse(Domain.Entities.Lesson lesson)
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
                IsLocked = !lesson.IsPreview
               
            },
            LessonType.PDF => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                IsLocked= !lesson.IsPreview
               
            },
            LessonType.Article => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                IsLocked =!lesson.IsPreview
               
            },
            _ => throw new InvalidOperationException($"Unsupported lesson type: {lesson.LessonType}")
        };
    }
}
