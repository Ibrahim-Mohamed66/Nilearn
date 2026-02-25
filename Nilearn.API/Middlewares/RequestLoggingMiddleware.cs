using System.Text;
using System.Text.Json;
namespace Nilearn.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private const int MaxBodyLength = 1000; // Avoid logging huge payloads

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Read and log the request
            string requestBody = await ReadRequestBody(context.Request);
            requestBody = MaskSensitiveData(requestBody);

            _logger.LogInformation("HTTP Request {Method} {Path} {QueryString} {Body}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                requestBody);

            // Capture the response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context); // Let exception middleware handle errors

            // Read and log response
            string responseBodyText = await ReadResponseBody(context.Response);
            _logger.LogInformation("HTTP Response {StatusCode} {ElapsedMilliseconds}ms {ResponseBody}",
                context.Response.StatusCode,
                context.Response.Headers.ContainsKey("X-Elapsed-Milliseconds")
                    ? context.Response.Headers["X-Elapsed-Milliseconds"].ToString()
                    : "N/A",
                responseBodyText);

            // Copy the response back
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (body.Length > MaxBodyLength)
                body = body.Substring(0, MaxBodyLength) + "...[truncated]";

            return string.IsNullOrWhiteSpace(body) ? "{}" : body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
            string text = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            if (text.Length > MaxBodyLength)
                text = text.Substring(0, MaxBodyLength) + "...[truncated]";

            return string.IsNullOrWhiteSpace(text) ? "{}" : text;
        }

        private string MaskSensitiveData(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return "{}";

            try
            {

                var jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>( body,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                       );


                if (jsonObj == null) return body;

                // List of sensitive fields to mask
                string[] sensitiveFields = { "password", "token", "access_token", "refresh_token", "creditCard", "ssn" };

                foreach (var field in sensitiveFields)
                {
                    if (jsonObj.ContainsKey(field))
                    {
                        jsonObj[field] = "****"; // Mask the value
                    }
                }

                return JsonSerializer.Serialize(jsonObj);
            }
            catch
            {

                return body;
            }
        }
    }
}