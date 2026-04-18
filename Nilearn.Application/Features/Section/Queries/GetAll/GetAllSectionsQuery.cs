using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;


namespace Nilearn.Application.Features.Section.Queries.GetAll;

public sealed record GetAllSectionsQuery(int CourseId) : IRequest<Result<List<SectionResponse>>>;

