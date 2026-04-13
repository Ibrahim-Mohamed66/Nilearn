using FluentValidation;
using Nilearn.Application.Features.Course.Queries.GetById;

namespace Nilearn.Application.Features.Course.Queries.GetById;

public sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0.");
    }
}
