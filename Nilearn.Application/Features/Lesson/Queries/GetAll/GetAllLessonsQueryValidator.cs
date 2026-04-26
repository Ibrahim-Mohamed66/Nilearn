using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Queries.GetAll;

public class GetAllLessonsQueryValidator : AbstractValidator<GetAllLessonsQuery>
{
    public GetAllLessonsQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than 0.");
    }
}
