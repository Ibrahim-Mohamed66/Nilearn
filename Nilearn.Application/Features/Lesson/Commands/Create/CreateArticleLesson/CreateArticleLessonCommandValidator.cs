using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Commands.Create.CreateArticleLesson;

public sealed class CreateArticleLessonCommandValidator : AbstractValidator<CreateArticleLessonCommand>
{
    public CreateArticleLessonCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than 0.");

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");
    }
}
