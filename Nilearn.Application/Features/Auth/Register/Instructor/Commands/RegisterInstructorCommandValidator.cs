using FluentValidation;

namespace Nilearn.Application.Features.Auth.Register.Instructor.Commands
{
    internal sealed class RegisterInstructorCommandValidator : AbstractValidator<RegisterInstructorCommand>
    {
        public RegisterInstructorCommandValidator() 
        {
            RuleFor(x => x.InstructorRequestDto.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            RuleFor(x => x.InstructorRequestDto.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
            RuleFor(x => x.InstructorRequestDto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.InstructorRequestDto.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.InstructorRequestDto.ConfirmPassword)
                .Equal(x => x.InstructorRequestDto.Password).WithMessage("Passwords do not match.");
            RuleFor(x => x.InstructorRequestDto.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Date of birth must be in the past.");
            RuleFor(x => x.InstructorRequestDto.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");
            RuleFor(x => x.InstructorRequestDto.Headline)
                .MaximumLength(100).WithMessage("Headline cannot exceed 100 characters.");
            RuleFor(x => x.InstructorRequestDto.WebsiteUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.InstructorRequestDto.WebsiteUrl))
                .WithMessage("Invalid website URL format.");
            RuleFor(x => x.InstructorRequestDto.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrWhiteSpace(x.InstructorRequestDto.PhoneNumber))
                .WithMessage("Invalid phone number format.");

        }
    }
}
