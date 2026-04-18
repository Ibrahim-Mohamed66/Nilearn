using FluentValidation;


namespace Nilearn.Application.Features.Section.Queries.GetAll;

public sealed class GetAllSectionsQueryValidator : AbstractValidator<GetAllSectionsQuery>
{
    public GetAllSectionsQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId is not valid.");
    }
}
