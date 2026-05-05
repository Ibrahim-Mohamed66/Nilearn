using FluentValidation;

namespace Nilearn.Application.Features.Reviews.Queries.GetMyReview;

public sealed class GetMyReviewQueryValidator : AbstractValidator<GetMyReviewQuery>
{
    public GetMyReviewQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Course ID must be greater than 0.");
    }
}
