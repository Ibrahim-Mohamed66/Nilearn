using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Payments.Queries.GetStudentPayments;
using Nilearn.Domain.Enums;
using System.Security.Claims;
using Nilearn.Application.Common.Exceptions;

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

            var query = new GetStudentPaymentsQuery(userId, pageNumber, pageSize, status);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandlePaymobWebhook([FromQuery] string hmac,CancellationToken cancellationToken)
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

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

            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
