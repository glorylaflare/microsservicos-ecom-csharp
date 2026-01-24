using Payment.Application.Consumer;
namespace Payment.Api.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddTransient<StockReservationResultConsumer>();
        return services;
    }
}