using FluentValidation;

namespace Nilearn.Application.Features.Wallets.Queries.GetInstructorWallet;

public class GetInstructorWalletQueryValidator : AbstractValidator<GetInstructorWalletQuery>
{
    public GetInstructorWalletQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
