using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Register.Student.DTOs;


namespace Nilearn.Application.Features.Auth.Register.Student.Commands;

public sealed record RegisterStudentCommand(RegisterStudentRequestDto registerRequestDto) : IRequest<Result<string>>;
