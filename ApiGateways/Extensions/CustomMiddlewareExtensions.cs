using Polly.CircuitBreaker;

namespace ApiGateways.Extensions
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        var sub = context.User.FindFirst("sub")?.Value;

                        if (!string.IsNullOrEmpty(sub))
                        {
                            context.Request.Headers["x-user-id"] = sub;
                        }
                    }

                    await next();
                }
                catch (BrokenCircuitException)
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Service is temporarily unavailable. Please try again later.");
                }
            });

            return app;
        }
    }
}
