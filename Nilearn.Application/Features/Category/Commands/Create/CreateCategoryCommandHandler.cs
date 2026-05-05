using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Domain.Interfaces;
using System.Xml.Linq;


namespace Nilearn.Application.Features.Category.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand,Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger)
    {

        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<string>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category with name: {Name}", request.Name);
        var existing = await _unitOfWork.CategoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if(existing != null)
        {
            _logger.LogWarning("Category with name '{Name}' already exists.", request.Name);
            throw new ConflictException("Category", "Category with the same name already exists.");
        }

        var category = new Domain.Entities.Category
        {
            Name = request.Name,
            Description = request.Description,
            IconClass = request.IconClass,
            IsActive = request.IsActive,

        };

        await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category '{Name}' created with success.", category.Name);
        return Result<string>.SuccessResponse(message:"Category created successfully.");
    }

}
