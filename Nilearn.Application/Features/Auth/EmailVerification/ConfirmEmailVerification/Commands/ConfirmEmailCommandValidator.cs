using FluentValidation;

namespace Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.Commands;

internal sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.emailRequestDto.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.emailRequestDto.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}