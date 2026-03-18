using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Register.Instructor.DTOs;

namespace Nilearn.Application.Features.Auth.Register.Instructor.Commands;

public sealed record RegisterInstructorCommand(InstructorRequestDto InstructorRequestDto) : IRequest<Result<string>>;

