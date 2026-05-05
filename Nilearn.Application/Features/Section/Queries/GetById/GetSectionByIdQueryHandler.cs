using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Section.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Section.Queries.GetById;

internal sealed class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSectionByIdQueryHandler> _logger;

    public GetSectionByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetSectionByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SectionResponse>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching section | SectionId: {SectionId} | CourseId: {CourseId}",
            request.Id, request.CourseId);

        var courseExists = await _unitOfWork.CourseRepository.AnyAsync(request.CourseId, cancellationToken);
        if (!courseExists)
        {
            _logger.LogWarning("Course not found | CourseId: {CourseId}", request.CourseId);
            throw new NotFoundException("Course", request.CourseId);
        }

        var section = await _unitOfWork.SectionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (section is null || section.CourseId != request.CourseId)
        {
            _logger.LogWarning(
                "Section not found | SectionId: {SectionId} | CourseId: {CourseId}",
                request.Id, request.CourseId);
            throw new NotFoundException("Section", request.Id);
        }

        var response = new SectionResponse(
            section.Id,
            section.Title,
            section.Description,
            section.Order,
            section.CourseId
        );

        _logger.LogInformation(
            "Section retrieved successfully | SectionId: {SectionId}",
            section.Id);

        return Result<SectionResponse>.SuccessResponse(response, "Section retrieved successfully.");
    }
}
