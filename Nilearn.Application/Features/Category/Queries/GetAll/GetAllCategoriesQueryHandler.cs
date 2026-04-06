using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Category.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Category.Queries.GetAll;

internal sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<PagedResponse<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCategoriesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PagedResponse<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching categories: Page {Page}, Size {Size}", request.PageNumber, request.PageSize);

        try
        {
            // Fetch paged categories from repository
            var pagedCategories = await _unitOfWork.CategoryRepository
                .GetPagedCategoriesAsync(request.PageNumber, request.PageSize, cancellationToken);

            // Map to DTOs
            var pagedResult = new PagedResponse<CategoryDto>
            {
                Items = pagedCategories.Items
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IconClass = c.IconClass,
                        IsActive = c.IsActive
                    })
                    .ToList(), 
                PageNumber = pagedCategories.PageNumber,
                PageSize = pagedCategories.PageSize,
                TotalCount = pagedCategories.TotalCount
            };

            _logger.LogInformation("Successfully retrieved {Count} categories out of {TotalCount}"
                , pagedResult.Items.Count, pagedResult.TotalCount);

            return new Result<PagedResponse<CategoryDto>>
            {
                Success = true,
                Data = pagedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories for page {Page} with size {Size}", 
                request.PageNumber, request.PageSize);

            return new Result<PagedResponse<CategoryDto>>
            {
                Success = false,
                Message = "Failed to fetch categories. Please try again later."
            };
        }
    }
}