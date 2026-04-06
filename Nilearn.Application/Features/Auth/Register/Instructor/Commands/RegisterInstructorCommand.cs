using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Auth.Register.Instructor.Commands;

public sealed record RegisterInstructorCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber,
    DateOnly DateOfBirth,
    string Bio,
    string? Headline,
    string? WebsiteUrl) : IRequest<Result<string>>;

