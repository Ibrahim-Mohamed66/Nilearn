using FluentValidation;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviews;

public sealed class GetCourseReviewsQueryValidator : AbstractValidator<GetCourseReviewsQuery>
{
    public GetCourseReviewsQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Course ID must be greater than 0.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
    }
}
