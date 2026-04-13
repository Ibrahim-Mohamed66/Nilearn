using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Course.Queries.GetPaged;

public sealed record GetCoursePagedQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResponse<CourseDto>>>;
