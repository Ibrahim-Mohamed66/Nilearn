using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Course.DTOs;
using Nilearn.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Nilearn.Application.Features.Course.Queries.GetByInstructorId
{
    internal sealed class GetCoursesByInstructorIdQueryHandler
        : IRequestHandler<GetCoursesByInstructorIdQuery, Result<List<CourseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;
        private readonly ILogger<GetCoursesByInstructorIdQueryHandler> _logger;

        public GetCoursesByInstructorIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMediaService mediaService,
            ILogger<GetCoursesByInstructorIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<Result<List<CourseDto>>> Handle(GetCoursesByInstructorIdQuery request, CancellationToken cancellationToken)
        {
            var instructorId = await _unitOfWork.InstructorRepository.GetIdByUserIdAsync(request.UserId);
            if (instructorId is null)
            {
                _logger.LogWarning("Instructor not found for user ID {UserId}", request.UserId);
                throw new NotFoundException("Instructor", request.UserId);
            }

            var courses = await _unitOfWork.CourseRepository.GetByInstructorId(instructorId.Value).ToListAsync(cancellationToken);

            var courseDtos = courses.Select(course => new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                ThumbnailUrl = _mediaService.GetImageUrl(course.ThumbnailPublicId),
                CategoryName = course.Category.Name,
                InstructorName = $"{course.Instructor.User.FirstName} {course.Instructor.User.LastName}",
                Price = course.Price,
                IsPublished = course.IsPublished,
                CreatedAt = course.CreatedAt
            }).ToList();

            return Result<List<CourseDto>>.SuccessResponse(courseDtos, "Courses retrieved successfully.");
        }
    }
}