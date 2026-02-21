using Order.Application.Consumers;
using Order.Infra.Projectors;
namespace Order.Api.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddTransient<StockReservationResultConsumer>();
        services.AddTransient<PaymentUpdatedConsumer>();

        #region MongoDb projections
        services.AddTransient<OrderCreatedProjector>();
        services.AddTransient<OrderUpdatedProjector>();
        services.AddTransient<UserCreatedProjector>();
        services.AddTransient<PaymentUpdatedProjector>();
        #endregion

        return services;
    }
}