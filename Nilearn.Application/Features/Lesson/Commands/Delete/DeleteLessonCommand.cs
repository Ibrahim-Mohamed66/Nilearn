using MediatR;
using Nilearn.Application.Common;


namespace Nilearn.Application.Features.Lesson.Commands.Delete;

public sealed record DeleteLessonCommand(int Id, int SectionId, string UserId) : IRequest<Result<string>>;

