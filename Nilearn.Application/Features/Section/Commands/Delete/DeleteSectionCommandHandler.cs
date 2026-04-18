using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
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

        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            _logger.LogWarning("Course not found | CourseId: {CourseId}", request.CourseId);
            return Result<string>.FailureResponse(message: "Course not found.");
        }

        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
        if (instructorId is null)
        {
            _logger.LogWarning("Instructor not found | UserId: {UserId}", request.UserId);
            return Result<string>.FailureResponse(
                    ["Instructor not found."],
                    "Failed to delete section.");
        }

        if (course.InstructorId != instructorId)
        {
            _logger.LogWarning(
                "Unauthorized access | CourseId: {CourseId} | UserId: {UserId}",
                request.CourseId, request.UserId);
            return Result<string>.FailureResponse(
                    ["Unauthorized access."],
                    "Failed to delete section.");
        }

        var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (section is null)
        {
            _logger.LogWarning("Section not found | SectionId: {SectionId}", request.Id);
            return Result<string>.FailureResponse(message: "Section not found.");
        }

        if (section.CourseId != request.CourseId)
        {
            _logger.LogWarning(
                "Section does not belong to course | SectionId: {SectionId} | CourseId: {CourseId}",
                request.Id, request.CourseId);
            return Result<string>.FailureResponse(
                    ["Section does not belong to the specified course."],
                    "Failed to delete section.");
        }
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {

            var deleted = await _unitOfWork.SectionRepository.DeleteAsync(request.Id, cancellationToken);
            if (!deleted)
            {
               
                return Result<string>.FailureResponse(message: "Failed to delete section.");
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
