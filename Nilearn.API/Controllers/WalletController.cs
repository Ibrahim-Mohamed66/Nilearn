using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Features.Wallets.Queries.GetInstructorWallet;
using Nilearn.Application.Features.Wallets.Queries.GetInstructorTransactions;
using Nilearn.Application.Features.Wallets.Queries.GetInstructorEarningsSummary;
using Nilearn.Application.Features.Wallets.Queries.GetPlatformRevenue;
using System.Security.Claims;

namespace Nilearn.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my-wallet")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetMyWallet(CancellationToken cancellationToken)
    {
        var userId =  GetUserId();
        if (userId == null) return Unauthorized();

        var query = new GetInstructorWalletQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("transactions")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetMyTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var userId =  GetUserId();
        if (userId == null) return Unauthorized();

        var query = new GetInstructorTransactionsQuery(userId, pageNumber, pageSize, startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("earnings-summary")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetMyEarningsSummary(CancellationToken cancellationToken)
    {
        var userId =  GetUserId();
        if (userId == null) return Unauthorized();

        var query = new GetInstructorEarningsSummaryQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("platform-revenue")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> GetPlatformRevenue(CancellationToken cancellationToken)
    {
        var query = new GetPlatformRevenueQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    private string? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return null;

        return userId;
    }
}
