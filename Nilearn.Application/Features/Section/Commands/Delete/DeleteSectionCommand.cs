using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Section.Commands.Delete;

public sealed record DeleteSectionCommand(int Id, int CourseId, string? UserId) : IRequest<Result<string>>;
