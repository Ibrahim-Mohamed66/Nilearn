using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Auth.Register.Student.Commands;

public sealed record RegisterStudentCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string PhoneNumber,
    DateOnly DateOfBirth): IRequest<Result<string>>;
