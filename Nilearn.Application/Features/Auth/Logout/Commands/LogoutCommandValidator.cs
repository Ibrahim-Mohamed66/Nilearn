using FluentValidation;

namespace Nilearn.Application.Features.Auth.Logout.Commands
{
    public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator() 
        {
            RuleFor(x => x.token)
                .NotEmpty();
                   
        }
    }
}
