using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Course.DTOs;


namespace Nilearn.Application.Features.Course.Queries.GetByInstructorId;

public sealed record GetCoursesByInstructorIdQuery(string UserId) : IRequest<Result<List<CourseDto>>>;

