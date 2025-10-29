using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace BuildingBlocks.Observability.Middlewares;

public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandleMiddleware(RequestDelegate next)
    {
        _next = next;
        _logger = Log.ForContext<ErrorHandleMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.Error(exception, $"An unhandled exception has occurred while processing the request. CorrelationId={context.TraceIdentifier}");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = JsonSerializer.Serialize(new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            CorrelationId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(response);
    }
}
