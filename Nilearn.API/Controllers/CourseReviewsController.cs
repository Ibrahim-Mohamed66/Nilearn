using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.API.Requests;
using Nilearn.Application.Features.Reviews.Commands.CreateReview;
using Nilearn.Application.Features.Reviews.Commands.UpdateReview;
using Nilearn.Application.Features.Reviews.Commands.DeleteReview;
using Nilearn.Application.Features.Reviews.Queries.GetCourseReviews;
using Nilearn.Application.Features.Reviews.Queries.GetCourseReviewSummary;
using Nilearn.Application.Features.Reviews.Queries.GetMyReview;
using System.Security.Claims;

namespace Nilearn.API.Controllers;

[Route("api/courses/{courseId:int}/reviews")]
[ApiController]
[Authorize]
public class CourseReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpPost]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> CreateReview(
        int courseId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new CreateReviewCommand(userId, courseId, request.Rating, request.Comment);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetMyReview), new { courseId }, result);
    }

   
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseReviews(
        int courseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseReviewsQuery(courseId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

   
    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewSummary(
        int courseId,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseReviewSummaryQuery(courseId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

   
    [HttpGet("me")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> GetMyReview(
        int courseId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var query = new GetMyReviewQuery(userId, courseId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

   
    [HttpPut("me")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> UpdateMyReview(
        int courseId,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new UpdateReviewCommand(userId, courseId, request.Rating, request.Comment);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    
    [HttpDelete("me")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> DeleteMyReview(
        int courseId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new DeleteReviewCommand(userId, courseId);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    private string? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return null;
        return userId;
    }
}
