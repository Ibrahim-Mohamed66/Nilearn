using FluentValidation;

namespace Nilearn.Application.Features.Course.Queries.GetForUpdate;

public sealed class GetCourseForUpdateQueryValidator : AbstractValidator<GetCourseForUpdateQuery>
{
    public GetCourseForUpdateQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0.");
    }
}
