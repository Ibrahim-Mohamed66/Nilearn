using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Queries.GetById;

public sealed record GetLessonByIdQuery(int Id, string? UserId) : IRequest<Result<LessonResponse>>;
