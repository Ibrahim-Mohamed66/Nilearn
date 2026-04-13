using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Course.DTOs;

namespace Nilearn.Application.Features.Course.Queries.GetById;

public sealed record GetCourseByIdQuery(int Id) : IRequest<Result<CourseDto?>>;
