using FluentValidation;

namespace Nilearn.Application.Features.Reviews.Queries.GetReviewById;

public sealed class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0).WithMessage("Review ID must be greater than 0.");
    }
}
