using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Course.Queries.GetPaged;

public sealed record GetCoursePagedQuery(
    int PageNumber, 
    int PageSize, 
    string? SearchTerm = null, 
    string? CategoryName = null, 
    string? InstructorName = null) : IRequest<Result<PagedResponse<CourseDto>>>;
