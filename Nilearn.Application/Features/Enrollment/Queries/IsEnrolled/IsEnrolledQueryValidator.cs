using FluentValidation;


namespace Nilearn.Application.Features.Enrollment.Queries.IsEnrolled
{
    public sealed class IsEnrolledQueryValidator : AbstractValidator<IsEnrolledQuery>
    {
        public IsEnrolledQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course ID must not be empty.");
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID must not be empty.");
        }
    }
}
