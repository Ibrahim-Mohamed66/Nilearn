using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Category.DTOs;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Category.Queries.GetAll;

public sealed record GetAllCategoriesQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResponse<CategoryDto>>>;

