using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Category.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category with id: {Id}", request.Id);

        var existing = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<string>.FailureResponse(
                new List<string> { "Category not found." },
                "Category not found.");
        }

        await _unitOfWork.CategoryRepository.DeleteCategoryAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category with id {Id} deleted with success.", request.Id);
        return Result<string>.SuccessResponse("Category deleted successfully.");
    }
}
