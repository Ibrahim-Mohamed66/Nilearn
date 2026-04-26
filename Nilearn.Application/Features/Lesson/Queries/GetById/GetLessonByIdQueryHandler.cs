using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
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
            return Result<LessonResponse>.FailureResponse(message: "Lesson not found.");
        }

        var response = MapToResponse(lesson);

        _logger.LogInformation("Successfully fetched lesson with ID: {LessonId}", request.Id);
        return Result<LessonResponse>.SuccessResponse(response, "Lesson fetched successfully.");
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

            },
            LessonType.PDF => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,

            },
            LessonType.Article => new LessonResponse
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                Description = lesson.Description,
                SectionId = lesson.SectionId,
                LessonType = lesson.LessonType,
                
            },
            _ => throw new InvalidOperationException($"Unsupported lesson type: {lesson.LessonType}")
        };
    }
}
