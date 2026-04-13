using FluentValidation;

namespace Nilearn.Application.Features.Course.Commands.Update;

internal sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId is required.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId is required.");
        
        When(x => x.Thumbnail != null, () =>
        {
            RuleFor(x => x.Thumbnail!)
                .Must(x => x.Content != null && x.Content.Length > 0).WithMessage("Thumbnail content is required.")
                .Must(x => !string.IsNullOrEmpty(x.FileName)).WithMessage("Thumbnail file name is required.")
                .Must(x => !string.IsNullOrEmpty(x.ContentType)).WithMessage("Thumbnail content type is required.");
        });

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0.");
    }
}
