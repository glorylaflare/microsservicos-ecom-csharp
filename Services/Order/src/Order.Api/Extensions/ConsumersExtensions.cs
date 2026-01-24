using Order.Application.Consumers;
using Order.Infra.Projectors;
namespace Order.Api.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddScoped<StockReservationResultConsumer>();

        #region MongoDb projections
        services.AddScoped<OrderCreatedProjector>();
        services.AddScoped<OrderUpdatedProjector>();
        #endregion

        return services;
    }
}