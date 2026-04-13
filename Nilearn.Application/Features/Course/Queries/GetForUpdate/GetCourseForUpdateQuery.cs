using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Course.DTOs;

namespace Nilearn.Application.Features.Course.Queries.GetForUpdate;

public sealed record GetCourseForUpdateQuery(int Id) : IRequest<Result<CourseForUpdateDto?>>;
