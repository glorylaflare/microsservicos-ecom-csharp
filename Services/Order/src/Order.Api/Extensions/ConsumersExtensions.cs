using Order.Application.Consumers;
using Order.Infra.Projectors;
namespace Order.Api.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddTransient<StockReservationResultConsumer>();

        #region MongoDb projections
        services.AddTransient<OrderCreatedProjector>();
        services.AddTransient<OrderUpdatedProjector>();
        #endregion

        return services;
    }
}