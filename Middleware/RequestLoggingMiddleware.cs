using System.Diagnostics;

namespace BlogApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var method = context.Request.Method;
            var path = context.Request.Path;
            var ip = context.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                "Incoming request: {Method} {Path} from {IP}",
                method,
                path,
                ip
            );

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;

                _logger.LogInformation(
                    "Request finished: {Method} {Path} → {StatusCode} in {Elapsed} ms",
                    method,
                    path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
    }
}