using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Category.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    string? IconClass,
    bool IsActive) : IRequest<Result<string>>;

