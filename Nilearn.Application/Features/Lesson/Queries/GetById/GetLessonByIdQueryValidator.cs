using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Queries.GetById;

public class GetLessonByIdQueryValidator : AbstractValidator<GetLessonByIdQuery>
{
    public GetLessonByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
