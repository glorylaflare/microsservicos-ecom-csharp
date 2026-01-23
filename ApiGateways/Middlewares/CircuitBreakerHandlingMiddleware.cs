using Polly.CircuitBreaker;
using Serilog;
namespace ApiGateways.Middlewares;
public class CircuitBreakerHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public CircuitBreakerHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BrokenCircuitException)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync("Service is temporarily unavailable. Please try again later.");
            Log.Warning("[WARN] A request was blocked due to an open circuit breaker.");
        }
    }
}
public static class CircuitBreakerHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCircuitBreakerHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CircuitBreakerHandlingMiddleware>();
    }
}