using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Category.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(int Id) : IRequest<Result<string>>;
