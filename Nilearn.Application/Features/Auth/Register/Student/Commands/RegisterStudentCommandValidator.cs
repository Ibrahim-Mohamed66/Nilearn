using FluentValidation;

namespace Nilearn.Application.Features.Auth.Register.Student.Commands
{
    public class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
    {
        public RegisterStudentCommandValidator()
        {
            RuleFor(x => x.registerRequestDto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.registerRequestDto.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.registerRequestDto.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.registerRequestDto.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.registerRequestDto.FirstName)
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.registerRequestDto.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.registerRequestDto.StudentNumber)
                .NotEmpty().WithMessage("Student number is required.")
                .Matches(@"^\d{9}$").WithMessage("Student number must be exactly 9 digits."); 

            RuleFor(x => x.registerRequestDto.CurrentLevel)
                .InclusiveBetween(1, 5).WithMessage("Current level must be between 1 and 5."); 
        }
    }
}