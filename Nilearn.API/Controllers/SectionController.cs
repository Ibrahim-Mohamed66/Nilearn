using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Features.Lesson.Queries.GetAll;
using Nilearn.Application.Features.Section.Commands.Create;
using Nilearn.Application.Features.Section.Commands.Delete;
using Nilearn.Application.Features.Section.Commands.Update;
using Nilearn.Application.Features.Section.Queries.GetById;
using System.Security.Claims;

namespace Nilearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<IActionResult> CreateSectionAsync([FromBody] CreateSectionCommand command, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            command = command with { UserId = userId };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<IActionResult> UpdateSectionAsync(int id, [FromBody] UpdateSectionCommand command, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            command = command with { Id = id, UserId = userId };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<IActionResult> DeleteSectionAsync(int id, [FromQuery] int courseId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var command = new DeleteSectionCommand(id, courseId, userId);
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSectionByIdAsync(int id, [FromQuery] int courseId, CancellationToken cancellationToken)
        {
            var query = new GetSectionByIdQuery(id, courseId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("{sectionId:int}/lessons")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllLessonsBySectionId(int sectionId, CancellationToken cancellationToken = default)
        {

            var query = new GetAllLessonsQuery(sectionId);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
