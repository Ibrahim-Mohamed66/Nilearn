using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.API.Requests;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.Commands.Create.CreateArticleLesson;
using Nilearn.Application.Features.Lesson.Commands.Create.CreatePdfLesson;
using Nilearn.Application.Features.Lesson.Commands.Create.CreateVideoLesson;
using Nilearn.Application.Features.Lesson.Commands.Update.UpdatePdfLesson;
using Nilearn.Application.Features.Lesson.Commands.Update.UpdateArticleLesson;
using Nilearn.Application.Features.Lesson.Commands.Update.UpdateVideoLesson;
using Nilearn.Application.Features.Lesson.Queries.GetById;
using Nilearn.Application.Features.Lesson.Queries.GetAll;
using System.Security.Claims;
using Nilearn.Application.Features.Lesson.Commands.Delete;
using Nilearn.Application.Common.Exceptions;

namespace Nilearn.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LessonController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("article")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> CreateArticleLesson([FromBody] CreateArticleLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var command = new CreateArticleLessonCommand(
            request.Title,
            request.Description,
            request.SectionId,
            request.Order,
            request.IsPreview,
            userId,
            request.Content
        );

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetLessonById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("video")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> CreateVideoLesson([FromForm] CreateVideoLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

       

        await using var stream = request.VideoFile.OpenReadStream();

        var videoFileUpload = new FileUpload
        {
            Content = stream,
            FileName = request.VideoFile.FileName,
            ContentType = request.VideoFile.ContentType,
            Length = request.VideoFile.Length
        };

        var command = new CreateVideoLessonCommand(
            request.Title,
            request.Description,
            request.SectionId,
            request.Order,
            request.IsPreview,
            userId,
            videoFileUpload
        );

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetLessonById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("pdf")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> CreatePdfLesson([FromForm] CreatePdfLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        if (request.PdfFile == null || request.PdfFile.Length == 0)
        {
            throw new BadRequestException("PDF file is required.");
        } await using var stream = request.PdfFile.OpenReadStream();

        var pdfFileUpload = new FileUpload
        {
            Content = stream,
            FileName = request.PdfFile.FileName,
            ContentType = request.PdfFile.ContentType,
            Length = request.PdfFile.Length
        };

        var command = new CreatePdfLessonCommand(
            request.Title,
            request.Description,
            request.SectionId,
            request.Order,
            request.IsPreview,
            userId,
            pdfFileUpload
        );

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut("pdf/{id:int}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> UpdatePdfLesson(int id, [FromForm] UpdatePdfLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        FileUpload? pdfFileUpload = null;
        if (request.PdfFile is not null)
        {
            var stream = request.PdfFile.OpenReadStream();
            pdfFileUpload = new FileUpload
            {
                Content = stream,
                FileName = request.PdfFile.FileName,
                ContentType = request.PdfFile.ContentType,
                Length = request.PdfFile.Length
            };
        }

        try
        {
            var command = new UpdatePdfLessonCommand(
                id,
                request.Title,
                request.SectionId,
                request.Description,
                request.Order,
                request.IsPreview,
                userId,
                pdfFileUpload
            );

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        finally
        {
            if (pdfFileUpload?.Content is not null)
                await pdfFileUpload.Content.DisposeAsync();
        }
    }

    [HttpPut("video/{id:int}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> UpdateVideoLesson(int id, [FromForm] UpdateVideoLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        FileUpload? videoFileUpload = null;
        if (request.VideoFile is not null)
        {
            var stream = request.VideoFile.OpenReadStream();
            videoFileUpload = new FileUpload
            {
                Content = stream,
                FileName = request.VideoFile.FileName,
                ContentType = request.VideoFile.ContentType,
                Length = request.VideoFile.Length
            };
        }

        try
        {
            var command = new UpdateVideoLessonCommand(
                id,
                request.Title,
                request.SectionId,
                request.Description,
                request.Order,
                request.IsPreview,
                userId,
                videoFileUpload
            );

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        finally
        {
            if (videoFileUpload?.Content is not null)
                await videoFileUpload.Content.DisposeAsync();
        }
    }

    [HttpPut("article/{id:int}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> UpdateArticleLesson(int id, [FromBody] UpdateArticleLessonRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var command = new UpdateArticleLessonCommand(
            id,
            request.Title,
            request.SectionId,
            request.Description,
            request.Order,
            request.IsPreview,
            userId,
            request.Content
        );

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLessonById(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id <= 0)
        {
            throw new BadRequestException("Invalid lesson ID.");
        } 
        
       
        var query = new GetLessonByIdQuery(id, userId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllLessonsBySectionId([FromQuery] int sectionId, CancellationToken cancellationToken = default)
    {

        var query = new GetAllLessonsQuery(sectionId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> DeleteLesson(int id, [FromQuery] int sectionId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var command = new DeleteLessonCommand(id, sectionId, userId);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);  
    }

}
