using FluentValidation;
using Nilearn.Application.Features.Category.Commands.CreateCategory;

namespace Nilearn.Application.Features.Category.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.IconClass)
            .MaximumLength(100).WithMessage("IconClass must not exceed 100 characters.")
            .When(x => x.IconClass is not null);
    }
}