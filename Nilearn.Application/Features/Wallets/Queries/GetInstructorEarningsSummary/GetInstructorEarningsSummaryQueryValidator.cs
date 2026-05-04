using FluentValidation;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorEarningsSummary;

public class GetInstructorEarningsSummaryQueryValidator : AbstractValidator<GetInstructorEarningsSummaryQuery>
{
    public GetInstructorEarningsSummaryQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
