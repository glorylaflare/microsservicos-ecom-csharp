using CorrelationId.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace BuildingBlocks.Observability.Extensions;

public static class CustomLoggingExtensions
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        var customTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[37m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[36m",
            [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
            [ConsoleThemeStyle.LevelError] = "\x1b[31m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[31m"
        });

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj} CorrelationId={CorrelationId}{NewLine}{Exception}",
                theme: customTheme,
                applyThemeToRedirectedOutput: true)
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} CorrelationId={CorrelationId}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        });

        services.AddDefaultCorrelationId(options =>
        {
            options.IncludeInResponse = true;
            options.UpdateTraceIdentifier = true;
        });

        return services;
    }
}
