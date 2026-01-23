namespace BuildingBlocks.Messaging;

public interface IEventBus
{
    Task StartAsync(CancellationToken cancellationToken);
    Task PublishAsync<T>(T @event) where T : IntegrationEventBase;
    Task SubscribeAsync<T, TH>()
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>;
}