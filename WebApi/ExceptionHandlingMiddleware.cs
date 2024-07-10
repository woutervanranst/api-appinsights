using System.Diagnostics;
using System.Net;

namespace WebApi.Bff;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Log the Exception
            _logger.LogError(ex, ex.Message);

            // Get the operation ID
            var operationId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

            // Construct the response
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = ex.Message,
                operationId = operationId
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
