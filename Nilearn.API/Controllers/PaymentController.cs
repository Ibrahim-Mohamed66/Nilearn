using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Payments.Queries.GetStudentPayments;
using Nilearn.Domain.Enums;
using System.Security.Claims;

namespace Nilearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMediator _mediator;
        public PaymentController(IPaymentGateway paymentGateway, IMediator mediator)
        {
            _paymentGateway = paymentGateway;
            _mediator = mediator;
        }

        [HttpGet("my-payments")]
        [Authorize(Policy = "StudentOnly")]
        public async Task<IActionResult> GetMyPayments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PaymentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest(Result<string>.FailureResponse(message: "Page number and page size must be greater than zero."));

            if (pageSize > 100)
                return BadRequest(Result<string>.FailureResponse(message: "Page size cannot exceed 100."));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var query = new GetStudentPaymentsQuery(userId, pageNumber, pageSize, status);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandlePaymobWebhook([FromQuery] string hmac,CancellationToken cancellationToken)
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            try
            {
                var result = _paymentGateway.ValidateAndParseWebhook(payload, hmac);

                var command = new Nilearn.Application.Features.Payments.Commands.ProcessPaymentWebhookCommand(
                    result.TransactionId,
                    result.OrderId,
                    result.IsSuccess,
                    result.Amount,
                    result.Currency,
                    result.MerchantReferenceId,
                    result.EnrollmentId
                );

                var processResult = await _mediator.Send(command,cancellationToken);
                if (processResult.Success)
                {
                    return Ok();
                }
                return BadRequest(processResult.Errors);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
