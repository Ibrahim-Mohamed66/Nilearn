using FluentValidation;

namespace Nilearn.Application.Features.Section.Commands.Create;

public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Course Id is invalid");
    }
}
