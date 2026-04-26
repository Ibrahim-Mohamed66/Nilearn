using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdatePdfLesson;

public sealed class UpdatePdfLessonCommandValidator : AbstractValidator<UpdatePdfLessonCommand>
{
    private static readonly HashSet<string> AllowedExtensions = [".pdf"];
    private const long MaxFileSizeBytes = 100L * 1024 * 1024; // 100 MB

    public UpdatePdfLessonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Lesson Id must be greater than 0.");

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

        When(x => x.PdfFile is not null, () =>
        {
            RuleFor(x => x.PdfFile!.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .Must(name => AllowedExtensions.Contains(
                    Path.GetExtension(name).ToLowerInvariant()))
                .WithMessage($"Only the following file types are allowed: {string.Join(", ", AllowedExtensions)}.");

            RuleFor(x => x.PdfFile!.Content)
                .NotNull().WithMessage("PDF file content is required.")
                .Must(content => content.Length > 0)
                .WithMessage("PDF file cannot be empty.")
                .Must(content => content.Length <= MaxFileSizeBytes)
                .WithMessage($"PDF file must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB.");
        });
    }
}
