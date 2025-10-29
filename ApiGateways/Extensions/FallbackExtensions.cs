using Polly.CircuitBreaker;

namespace ApiGateways.Extensions
{
    public static class FallbackExtensions
    {
        public static IApplicationBuilder UseFallback(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
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
