using FluentValidation;
using Nilearn.Application.Features.Category.Queries.GetById;

namespace Nilearn.Application.Features.Category.Validators;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");
    }
}
