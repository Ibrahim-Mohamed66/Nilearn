using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Interfaces;


namespace Nilearn.Application.Features.Course.Commands.Delete;

internal sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly IMediaService _mediaService;
    private readonly IMediaJobScheduler _mediaJobScheduler;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCourseCommandHandler> logger, IMediaService mediaService, IMediaJobScheduler mediaJobScheduler)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediaService = mediaService;
        _mediaJobScheduler = mediaJobScheduler;
    }

    public async Task<Result<string>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId);
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.Id,cancellationToken);

        


        if (course == null)
        {
            _logger.LogWarning("Course with ID {CourseId} not found for deletion.", request.Id);
            throw new NotFoundException("Course", request.Id);
        }

        if (course.InstructorId != instructorId)
        {
            _logger.LogWarning(
                "Unauthorized delete attempt. User {UserId} tried to delete course {CourseId}.",
                request.UserId,
                request.Id);

            throw new ForbiddenAccessException("You are not allowed to delete this course.");
        }
        try
        {
            await _mediaJobScheduler.EnqueueDeleteImageAsync(course.ThumbnailPublicId);
            _logger.LogInformation("Deleted thumbnail for course with ID {CourseId} from media service.", request.Id);

        }
        catch (Exception)
        {
            _logger.LogError("Failed to delete thumbnail for course with ID {CourseId} from media service.", request.Id);

            throw;
        }

        var deleted = await _unitOfWork.CourseRepository.DeleteAsync(course.Id, cancellationToken);
        if (!deleted)
        {
            throw new BadRequestException("Failed to delete course.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Deleted course with ID {CourseId} from database.", request.Id);
        return Result<string>.SuccessResponse(message: "Course deleted successfully.");



    }
}
