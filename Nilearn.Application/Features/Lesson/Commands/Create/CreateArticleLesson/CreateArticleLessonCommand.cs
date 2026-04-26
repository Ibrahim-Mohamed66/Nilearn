using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Commands.Create.CreateArticleLesson;

public sealed record CreateArticleLessonCommand(
    string Title,
    string? Description,
    int SectionId,
    int Order,
    bool IsPreview,
    string UserId,
    string Content) : IRequest<Result<LessonResponse>>;
