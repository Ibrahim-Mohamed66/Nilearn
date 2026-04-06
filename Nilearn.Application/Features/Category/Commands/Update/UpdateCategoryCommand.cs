using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Category.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    int Id,
    string Name,
    string? Description,
    string? IconClass,
    bool IsActive) : IRequest<Result<string>>;
