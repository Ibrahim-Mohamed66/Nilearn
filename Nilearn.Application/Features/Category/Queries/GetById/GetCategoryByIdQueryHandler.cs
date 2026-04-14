using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Category.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Category.Queries.GetById;

internal sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching category with id: {Id}", request.Id);

        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDto>.FailureResponse(
                new List<string> { "Category not found." },
                "Category not found.");
        }

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IconClass = category.IconClass,
            IsActive = category.IsActive
        };

        _logger.LogInformation("Category '{Name}' retrieved successfully.", category.Name);
        return Result<CategoryDto>.SuccessResponse(dto, "Category retrieved successfully.");
    }
}
