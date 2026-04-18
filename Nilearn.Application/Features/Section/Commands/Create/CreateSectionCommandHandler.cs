using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Section.Commands.Create
{
    
    internal sealed class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSectionCommandHandler> _logger;

        public CreateSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSectionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<SectionResponse>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
               "Creating section | CourseId: {CourseId} | Order: {Order}",
               request.CourseId, request.Order);

            
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
            {
                _logger.LogWarning(
                    "Course not found | CourseId: {CourseId}",
                    request.CourseId);

                return Result<SectionResponse>.FailureResponse(
                    [ "Course not found." ],
                    "Failed to create section.");
            }
            var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId, cancellationToken);
            if (instructorId is null)
            {
                _logger.LogWarning("Instructor not found | UserId: {UserId}", request.UserId);

                return Result<SectionResponse>.FailureResponse(
                    ["Instructor not found."],
                    "Failed to create section.");
            }

            if (course.InstructorId != instructorId )
            {
                _logger.LogWarning(
                    "Unauthorized access | CourseId: {CourseId} | UserId: {UserId}",
                    request.CourseId, request.UserId);

                return Result<SectionResponse>.FailureResponse(
                    [ "Unauthorized access." ],
                    "Failed to create section.");
            }
            
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var maxOrder = await _unitOfWork.SectionRepository.GetMaxOrderAsync(request.CourseId, cancellationToken);

                var finalOrder = request.Order > maxOrder + 1
                            ? maxOrder + 1
                            : request.Order;
                if (finalOrder <= maxOrder)
                    await _unitOfWork.SectionRepository
                            .IncrementOrderFromAsync(request.CourseId, finalOrder, cancellationToken);
                var section = new Domain.Entities.Section
                {
                    Title = request.Title,
                    Description = request.Description,
                    Order = finalOrder,
                    CourseId = request.CourseId,
                    CreatedAt = DateTime.UtcNow,
                };
                await _unitOfWork.SectionRepository.AddAsync(section, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
               "Section created successfully | SectionId: {SectionId}",
               section.Id);


                var response = new SectionResponse(
                       section.Id,
                       section.Title,
                       section.Description,
                       section.Order,
                       section.CourseId
                   );
                return Result<SectionResponse>.SuccessResponse(response, "Section created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while creating section | CourseId: {CourseId} | UserId: {UserId}",
                    request.CourseId, request.UserId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
           

            
        }
    }
}
