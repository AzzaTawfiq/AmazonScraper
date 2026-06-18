using AmazonScraper.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AmazonScraper.Api.Middlewares
{
    public class ScraperExceptionHandler: IExceptionHandler
    {
        private readonly ILogger<ScraperExceptionHandler> _logger;

        public ScraperExceptionHandler(ILogger<ScraperExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // 1. Log the specific failure for internal diagnostic monitoring
            _logger.LogError(exception, "An error occurred while executing the Amazon scraping operation: {Message}", exception.Message);

            // 2. Map the scraping exception to appropriate HTTP Status Codes and details
            var (statusCode, title, detail) = exception switch
            {
                AmazonCaptchaException captchaEx => (
                    (int)HttpStatusCode.ServiceUnavailable,
                    "Bot Detection Block",
                    captchaEx.Message),

                ProductNotFoundException notFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    "Product Missing",
                    notFoundEx.Message),

                ProxyRotationException proxyEx => (
                    (int)HttpStatusCode.BadGateway,
                    "Proxy Network Failure",
                    proxyEx.Message),

                AmazonScraperException generalEx => (
                    (int)HttpStatusCode.InternalServerError,
                    "Scraper Engine Error",
                    generalEx.Message),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    "Please contact support if the issue persists.")
            };

            // 3. Build a structured ProblemDetails response to avoid leaking system stack traces
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            // Optionally attach a tracking/correlation trace ID
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            // 4. Set the HTTP context states and send the JSON output
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            // Return true to signal that this exception has been completely handled
            return true;
        }

    }
}
