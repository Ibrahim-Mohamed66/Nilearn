using FluentValidation;

namespace Nilearn.Application.Features.Auth.EmailVerification.SendEmailVerification.Commands;

public class SendEmailVerificationCommandValidator : AbstractValidator<SendEmailVerificationCommand>
{
    public SendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is Missing.");
    }
}
