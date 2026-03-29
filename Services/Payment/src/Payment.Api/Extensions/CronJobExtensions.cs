using Hangfire;
using Payment.Application.Interfaces;

namespace Payment.Api.Extensions;

public static class CronJobExtensions
{
    public static IServiceCollection AddCronJob(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
        );

        services.AddHangfireServer();

        return services;
    }

    public static IApplicationBuilder UseCronJob(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire");

        RecurringJob.AddOrUpdate<IPaymentExpirationService>(
            "expire-payments-job",
            service => service.ProcessExpiredPaymentsAsync(),
            "*/12 * * * *" 
        );

        RecurringJob.AddOrUpdate<IWebhookProcessorService>(
            "webhook-processor-job",
            service => service.ProcessWebhookAsync(),
            "*/10 * * * * *"
        );

        return app;
    }
}
