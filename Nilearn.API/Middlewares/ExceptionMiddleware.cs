using System.Net;
using System.Text.Json;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Domain.Exceptions;

namespace Nilearn.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", string.Join("; ", ex.Errors));
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Validation failed.", ex.Errors);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Bad request: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, ex.Errors);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                _logger.LogWarning("Forbidden access: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning("Conflict: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (PaymentGatewayException ex)
            {
                _logger.LogError(ex, "Payment gateway error: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.BadGateway, "Payment gateway error occurred.");
            }
            catch (PaymentAlreadyFinalizedException ex)
            {
                _logger.LogWarning("Payment already finalized: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access (system): {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteResponseAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message,
            IEnumerable<string>? errors = null)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message,
                errors = errors ?? Enumerable.Empty<string>()
            };

            var json = JsonSerializer.Serialize(response, JsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
