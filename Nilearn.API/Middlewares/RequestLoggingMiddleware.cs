using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            var stopwatch = Stopwatch.StartNew();

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
            stopwatch.Stop(); // Stop stopwatch after request finishes  
            string responseBodyText = await ReadResponseBody(context.Response);
            responseBodyText = MaskSensitiveData(responseBodyText);
            _logger.LogInformation("HTTP Response {StatusCode} {ElapsedMilliseconds}ms {ResponseBody}",
                context.Response.StatusCode,
                 stopwatch.ElapsedMilliseconds,
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
                var jsonNode = JsonNode.Parse(body);
                if (jsonNode == null) return body;

                // List of sensitive fields to mask (checked case-insensitively)
                string[] sensitiveFields = { "password", "token", "accesstoken", "accesstokens", "refreshtoken", "refreshtokens","confirmpassword" ,"paymenturl", "creditcard", "ssn" };


                MaskNode(jsonNode, sensitiveFields);

                return jsonNode.ToJsonString();
            }
            catch
            {
                return body;
            }
        }

        private void MaskNode(JsonNode node, string[] sensitiveFields)
        {
            if (node is JsonObject obj)
            {
                var properties = obj.ToList();
                foreach (var prop in properties)
                {
                    if (sensitiveFields.Contains(prop.Key.ToLowerInvariant()))
                    {
                        obj[prop.Key] = "****"; // Mask the value
                    }
                    else if (prop.Value != null)
                    {
                        MaskNode(prop.Value, sensitiveFields);
                    }
                }
            }
            else if (node is JsonArray arr)
            {
                foreach (var item in arr)
                {
                    if (item != null)
                    {
                        MaskNode(item, sensitiveFields);
                    }
                }
            }
        }
    }
}