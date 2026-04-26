using FluentValidation;

namespace Nilearn.Application.Features.Lesson.Commands.Delete
{
    public sealed class DeleteLessonCommandValidator : AbstractValidator<DeleteLessonCommand>
    {
        public DeleteLessonCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Lesson ID is required.")
                .GreaterThan(0);
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.")
                .GreaterThan(0);
            
        }
    }
}
