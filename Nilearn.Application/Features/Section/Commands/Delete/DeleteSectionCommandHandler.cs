using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Section.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Section.Commands.Delete;

internal sealed class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSectionCommandHandler> _logger;

    public DeleteSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSectionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting section | SectionId: {SectionId} | CourseId: {CourseId}", request.Id, request.CourseId);

        var isOwner = await _unitOfWork.CourseRepository.IsOwner(request.CourseId, request.UserId, cancellationToken);


        if (!isOwner)
        {
            _logger.LogWarning(
                "Unauthorized access | CourseId: {CourseId} | UserId: {UserId}",
                request.CourseId, request.UserId);

            throw new ForbiddenAccessException("You are not authorized to delete sections in this course.");
        }
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (section is null)
        {
            _logger.LogWarning("Section not found | SectionId: {SectionId}", request.Id);
            throw new NotFoundException("Section", request.Id);
        }
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {

            var deleted = await _unitOfWork.SectionRepository.DeleteAsync(request.Id, cancellationToken);
            if (!deleted)
            {
               
                throw new BadRequestException("Failed to delete section.");
            }
            


            await _unitOfWork.SectionRepository
                .DecrementOrderFromAsync(request.CourseId, section.Order + 1, cancellationToken);


            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Section deleted successfully | SectionId: {SectionId}", request.Id);
            return Result<string>.SuccessResponse(message: "Section deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while deleting section | SectionId: {SectionId} | CourseId: {CourseId}",
                request.Id, request.CourseId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        
    }
}
