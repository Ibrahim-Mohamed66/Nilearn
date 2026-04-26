using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdateVideoLesson;

public sealed class UpdateVideoLessonCommandValidator : AbstractValidator<UpdateVideoLessonCommand>
{
    private static readonly HashSet<string> AllowedExtensions = [".mp4", ".mov", ".avi", ".mkv", ".webm"];
    private static readonly HashSet<string> AllowedMimeTypes = ["video/mp4", "video/quicktime", "video/x-msvideo", "video/x-matroska", "video/webm"];
    private const long MaxFileSizeBytes = 2L * 1024 * 1024 * 1024; // 2GB

    public UpdateVideoLessonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Lesson Id must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId is required.");

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        When(x => x.VideoFile is not null, () =>
        {
            RuleFor(x => x.VideoFile!.FileName)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
                .WithMessage($"File must be a valid video format : {string.Join(", ", AllowedExtensions)}.");

            RuleFor(x => x.VideoFile!.ContentType)
                .Must(ct => AllowedMimeTypes.Contains(ct.ToLowerInvariant()))
                .WithMessage("Invalid video MIME type");

            RuleFor(x => x.VideoFile!.Content)
                .Must(content => content.Length > 0)
                .WithMessage("Video file cannot be empty")
                .Must(content => content.Length <= MaxFileSizeBytes)
                .WithMessage($"Video file must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");
        });
    }
}
