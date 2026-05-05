using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Section.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Section.Commands.Update;

internal sealed class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSectionCommandHandler> _logger;

    public UpdateSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSectionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SectionResponse>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var isOwner = await _unitOfWork.CourseRepository.IsOwner(request.CourseId, request.UserId, cancellationToken);


        if (!isOwner)
        {
            _logger.LogWarning(
                "Unauthorized access | CourseId: {CourseId} | UserId: {UserId}",
                request.CourseId, request.UserId);

            throw new ForbiddenAccessException("You are not authorized to update sections in this course.");
        }

        var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (section is null)
        {
            _logger.LogWarning("Section not found | SectionId: {SectionId}", request.Id);
            throw new NotFoundException("Section", request.Id);
        }


        if (section.CourseId != request.CourseId)
        {
            _logger.LogWarning(
                "Section does not belong to course | SectionId: {SectionId} | CourseId: {CourseId}",
                request.Id, request.CourseId);
            throw new BadRequestException("Section does not belong to the specified course.");
        }



        var maxOrder = await _unitOfWork.SectionRepository.GetMaxOrderAsync(request.CourseId, cancellationToken);

        var finalOrder = Math.Clamp(request.Order, 1, maxOrder);

        if (section.Order == finalOrder &&section.Title == request.Title &&
                section.Description == request.Description)
        {
            return Result<SectionResponse>.SuccessResponse(
                new SectionResponse(section.Id, section.Title, section.Description, section.Order, section.CourseId),
                "No changes detected.");
        }
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Handle order reordering if the order changed
            if (section.Order != finalOrder)
            {

                if (finalOrder < section.Order)
                {
                    // Moving up → shift others down
                    await _unitOfWork.SectionRepository
                        .IncrementOrderRangeAsync(request.CourseId, finalOrder, section.Order - 1, cancellationToken);
                }
                else if (finalOrder > section.Order)
                {
                    // Moving down → shift others up
                    await _unitOfWork.SectionRepository
                        .DecrementOrderRangeAsync(request.CourseId, section.Order + 1, finalOrder, cancellationToken);
                }

                section.Order = finalOrder;
            }

            section.Title = request.Title;
            section.Description = request.Description;

            _unitOfWork.SectionRepository.Update(section);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Section updated successfully | SectionId: {SectionId}",
                section.Id);

            var response = new SectionResponse(
                section.Id,
                section.Title,
                section.Description,
                section.Order,
                section.CourseId
            );

            return Result<SectionResponse>.SuccessResponse(response, "Section updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while updating section | SectionId: {SectionId} | CourseId: {CourseId}",
                request.Id, request.CourseId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
