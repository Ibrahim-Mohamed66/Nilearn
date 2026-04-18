using FluentValidation;

namespace Nilearn.Application.Features.Section.Queries.GetById;

public sealed class GetSectionByIdQueryValidator : AbstractValidator<GetSectionByIdQuery>
{
    public GetSectionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Section ID must be greater than 0.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0.");
    }
}
