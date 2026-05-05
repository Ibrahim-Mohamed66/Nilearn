using FluentValidation;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviewSummary;

public sealed class GetCourseReviewSummaryQueryValidator : AbstractValidator<GetCourseReviewSummaryQuery>
{
    public GetCourseReviewSummaryQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Course ID must be greater than 0.");
    }
}
