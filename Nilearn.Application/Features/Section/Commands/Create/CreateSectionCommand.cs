using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;


namespace Nilearn.Application.Features.Section.Commands.Create;

public sealed record CreateSectionCommand(
     string Title,
     string? Description,
     int Order,
     int CourseId,
     string? UserId
 ) : IRequest<Result<SectionResponse>>;
