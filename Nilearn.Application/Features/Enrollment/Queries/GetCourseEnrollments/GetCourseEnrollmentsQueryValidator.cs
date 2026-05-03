using FluentValidation;

namespace Nilearn.Application.Features.Enrollment.Queries.GetCourseEnrollments;

public sealed class GetCourseEnrollmentsQueryValidator : AbstractValidator<GetCourseEnrollmentsQuery>
{
    public GetCourseEnrollmentsQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");
    }
}
