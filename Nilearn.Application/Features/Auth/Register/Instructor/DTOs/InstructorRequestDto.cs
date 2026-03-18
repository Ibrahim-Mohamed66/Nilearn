namespace Nilearn.Application.Features.Auth.Register.Instructor.DTOs;

public record InstructorRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? WebsiteUrl { get; set; }
}