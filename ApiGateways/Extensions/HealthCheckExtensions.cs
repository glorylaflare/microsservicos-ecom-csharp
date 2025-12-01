using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ApiGateways.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthChecksService(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddUrlGroup(new Uri("http://localhost:5001/health"), name: "Order Service")
                .AddUrlGroup(new Uri("http://localhost:5002/health"), name: "Stock Service")
                .AddUrlGroup(new Uri("http://localhost:5003/health"), name: "User Service")
                .AddUrlGroup(new Uri("http://localhost:5004/health"), name: "Auth Service");

            return services;
        }

        public static IApplicationBuilder UseHealthChecksService(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new { 
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            exception = e.Value.Exception?.Message
                        })
                    };
                    await context.Response.WriteAsJsonAsync(response);
                }
            });

            return app;
        }
    }
}
