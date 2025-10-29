using BuildingBlocks.Observability.Middlewares;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Diagnostics;

namespace BuildingBlocks.Observability.Extensions;

public static class ObservabilityExtensions
{
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandleMiddleware>();

        app.UseCorrelationId();

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var correlationId = context.TraceIdentifier;

            Log.Information($"Handling request: {method} {path} CorrelationId={correlationId}");

            var watch = Stopwatch.StartNew();
            await next();
            watch.Stop();

            Log.Information($"Finished handling request: {method} {path} CorrelationId={correlationId} in {watch.ElapsedMilliseconds}ms");
        });

        return app;
    }
}
