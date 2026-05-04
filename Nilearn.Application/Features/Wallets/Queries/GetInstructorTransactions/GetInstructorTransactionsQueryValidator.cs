using FluentValidation;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorTransactions;

public class GetInstructorTransactionsQueryValidator : AbstractValidator<GetInstructorTransactionsQuery>
{
    public GetInstructorTransactionsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
