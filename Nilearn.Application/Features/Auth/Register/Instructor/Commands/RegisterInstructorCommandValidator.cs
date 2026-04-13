using FluentValidation;

namespace Nilearn.Application.Features.Auth.Register.Instructor.Commands
{
    public sealed class RegisterInstructorCommandValidator : AbstractValidator<RegisterInstructorCommand>
    {
        public RegisterInstructorCommandValidator() 
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Date of birth must be in the past.");
            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");
            RuleFor(x => x.Headline)
                .MaximumLength(100).WithMessage("Headline cannot exceed 100 characters.");
            RuleFor(x => x.WebsiteUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
                .WithMessage("Invalid website URL format.");
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Invalid phone number format.");

        }
    }
}
