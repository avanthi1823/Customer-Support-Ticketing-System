using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CSTS.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
          
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

           
            var response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Something went wrong. Please try again later.",
                Error = exception.Message
            };

            httpContext.Response.StatusCode = response.StatusCode;
            httpContext.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response);
            await httpContext.Response.WriteAsync(json, cancellationToken);

            return true; 
        }
    }
}