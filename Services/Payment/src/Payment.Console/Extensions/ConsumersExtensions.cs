using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Consumer;

namespace Payment.Console.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddScoped<StockReservationResultConsumer>();

        return services;
    }
}
