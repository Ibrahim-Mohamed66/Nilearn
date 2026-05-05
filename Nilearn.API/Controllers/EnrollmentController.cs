using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.API.Requests;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.Commands.Create;
using Nilearn.Application.Features.Enrollment.Queries.GetCourseEnrollments;
using Nilearn.Application.Features.Enrollment.Queries.GetStudentEnrollments;
using Nilearn.Application.Features.Enrollment.Queries.IsEnrolled;
using Nilearn.Domain.Enums;
using Nilearn.Application.Common.Exceptions;
using System.Security.Claims;

namespace Nilearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "StudentOnly")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            var result = await _mediator.Send(new CreateEnrollmentCommand(userId, request.CourseId), cancellationToken);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("my-enrollments")]
        [Authorize(Policy = "StudentOnly")]
        public async Task<IActionResult> GetMyEnrollments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] EnrollmentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new BadRequestException("Page number and page size must be greater than zero.");
            }
            if (pageSize > 100)
            {
                throw new BadRequestException("Page size cannot exceed 100.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var query = new GetStudentEnrollmentsQuery(userId, pageNumber, pageSize, status);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        [HttpGet("course/{courseId:int}")]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<IActionResult> GetCourseEnrollments(
            int courseId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] EnrollmentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new BadRequestException("Page number and page size must be greater than zero.");
            }
            if (pageSize > 100)
            {
                throw new BadRequestException("Page size cannot exceed 100.");
            }

            var query = new GetCourseEnrollmentsQuery(courseId, pageNumber, pageSize, status);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        [HttpGet("is-enrolled/{courseId:int}")]
        [Authorize(Policy = "StudentOnly")]
        public async Task<IActionResult> IsEnrolled(int courseId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var query = new IsEnrolledQuery(userId, courseId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
