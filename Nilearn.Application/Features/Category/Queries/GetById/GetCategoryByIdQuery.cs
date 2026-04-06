using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Category.DTOs;

namespace Nilearn.Application.Features.Category.Queries.GetById;

public sealed record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryDto>>;
