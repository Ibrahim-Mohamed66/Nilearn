using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Course.Commands.Delete;

public sealed record DeleteCourseCommand(int Id,string UserId) : IRequest<Result<string>>;
    
