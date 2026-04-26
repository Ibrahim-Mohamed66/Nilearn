using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Queries.GetAll;

public sealed record GetAllLessonsQuery(int SectionId) : IRequest<Result<List<LessonResponse>>>;
