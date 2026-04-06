using FluentValidation;
using Nilearn.Application.Features.Category.Commands.DeleteCategory;

namespace Nilearn.Application.Features.Category.Validators;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required.");
    }
}
