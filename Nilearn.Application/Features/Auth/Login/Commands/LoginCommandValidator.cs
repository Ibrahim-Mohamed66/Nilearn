using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Features.Auth.Login.Commands
{
    public class LoginCommandValidator :AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
