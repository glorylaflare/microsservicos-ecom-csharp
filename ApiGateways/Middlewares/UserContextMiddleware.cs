using Serilog;

namespace ApiGateways.Middlewares;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(sub))
            {
                context.Request.Headers["x-user-id"] = sub;
                Log.Information("Added x-user-id header with value {UserId}", sub);
            }
        }
        await _next(context);
    }
}

public static class UserContextMiddlewareExtensions
{
    public static IApplicationBuilder UseUserContextMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserContextMiddleware>();
    }
}
