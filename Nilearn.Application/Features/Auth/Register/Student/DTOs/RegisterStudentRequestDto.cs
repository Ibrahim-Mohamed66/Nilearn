using Nilearn.Domain.Enums;

namespace Nilearn.Application.Features.Auth.Register.Student.DTOs
{
    public class RegisterStudentRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string StudentNumber { get; set; }
        public int CurrentLevel { get; set; }
    }
}
