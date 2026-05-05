using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Features.Category.Commands.CreateCategory;
using Nilearn.Application.Features.Category.Commands.DeleteCategory;
using Nilearn.Application.Features.Category.Commands.UpdateCategory;
using Nilearn.Application.Features.Category.Queries.GetAll;
using Nilearn.Application.Features.Category.Queries.GetById;

namespace Nilearn.API.Controllers
{
    [Authorize(Policy = "AdminOrInstructor")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateCategory), result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id,[FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            return Ok(result);
        }
    }
}
