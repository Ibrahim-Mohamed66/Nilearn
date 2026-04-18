using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;

namespace Nilearn.Application.Features.Section.Queries.GetById;

public sealed record GetSectionByIdQuery(int Id, int CourseId) : IRequest<Result<SectionResponse>>;
