using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.API.Requests;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Features.Course.Commands.Create;
using Nilearn.Application.Features.Course.Commands.Delete;
using Nilearn.Application.Features.Course.Commands.Update;
using Nilearn.Application.Features.Course.Queries.GetById;
using Nilearn.Application.Features.Course.Queries.GetByInstructorId;
using Nilearn.Application.Features.Course.Queries.GetForUpdate;
using Nilearn.Application.Features.Course.Queries.GetPaged;
using Nilearn.Application.Features.Section.Queries.GetAll;
using System.Security.Claims;

namespace Nilearn.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> CreateCourse([FromForm] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        if (request.Thumbnail is null)
            return BadRequest(Result<string>.FailureResponse(message: "Thumbnail is required."));

        await using var stream = request.Thumbnail.OpenReadStream();

        var thumbnail = new FileUpload
        {
            Content = stream,
            FileName = request.Thumbnail.FileName,
            ContentType = request.Thumbnail.ContentType
        };

        var command = new CreateCourseCommand(
            request.Title,
            request.Description,
            request.CategoryId,
            userId,
            thumbnail,
            UploadPurpose.Thumbnail,
            request.Price,
            request.IsPublished
        );

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = result.Data!.CourseId },
            result
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> UpdateCourse(int id, [FromForm] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        FileUpload? thumbnail = null;
        Stream? thumbnailStream = null;

        try
        {
            if (request.Thumbnail is not null)
            {
                thumbnailStream = request.Thumbnail.OpenReadStream();
                thumbnail = new FileUpload
                {
                    Content = thumbnailStream,
                    FileName = request.Thumbnail.FileName,
                    ContentType = request.Thumbnail.ContentType
                };
            }

            var command = new UpdateCourseCommand(
                id,
                request.Title,
                request.Description,
                request.CategoryId,
                userId,
                thumbnail,
                UploadPurpose.Thumbnail,
                request.Price,
                request.IsPublished
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        finally
        {
            if (thumbnailStream != null)
            {
                await thumbnailStream.DisposeAsync();
            }
        }
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(Result<string>.FailureResponse(message: "Invalid course ID."));

        var query = new GetCourseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{id:int}/update")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetCourseForUpdate(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(Result<string>.FailureResponse(message: "Invalid course ID."));

        var query = new GetCourseForUpdateQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest(Result<string>.FailureResponse(message: "Page number and page size must be greater than zero."));

        if (pageSize > 50)
            return BadRequest(Result<string>.FailureResponse(message: "Page size cannot exceed 50."));

        var query = new GetCoursePagedQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("instructor-courses")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetCoursesByInstructorId(
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        if (string.IsNullOrEmpty(userId))
            return BadRequest(Result<string>.FailureResponse(message: "Instructor ID must be provided."));
        var query = new GetCoursesByInstructorIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);


    }


    [HttpGet("{courseId}/sections")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSectionsByCourseIdAsync([FromRoute] GetAllSectionsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);

    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrInstructor")]
    public async Task<IActionResult> DeleteCourse([FromRoute]int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var command = new DeleteCourseCommand(id,userId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}