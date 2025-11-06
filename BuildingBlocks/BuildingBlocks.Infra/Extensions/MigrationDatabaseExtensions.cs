using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;

namespace BuildingBlocks.Infra.Extensions;

public static class MigrationDatabaseExtensions
{
    public static async Task<IApplicationBuilder> AddMigrateDatabase<TContext>(this IApplicationBuilder app)
        where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<TContext>();

        try
        {
            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning(
                            exception,
                            "Retry {RetryCount} of migrating database for context {DbContextName} after {TimeSpan} seconds due to error: {ErrorMessage}",
                            retryCount,
                            typeof(TContext).Name,
                            timeSpan.TotalSeconds,
                            exception.Message);
                    }
                );

            await retry.ExecuteAsync(() =>
                context.Database.MigrateAsync()
            );

            Log.Information("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while migrating the database.");
            throw;
        }

        return app;
    }
}
