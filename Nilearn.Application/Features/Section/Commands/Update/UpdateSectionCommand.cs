using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;

namespace Nilearn.Application.Features.Section.Commands.Update;

public sealed record UpdateSectionCommand(
    int Id,
    string Title,
    string? Description,
    int Order,
    int CourseId,
    string? UserId
) : IRequest<Result<SectionResponse>>;
