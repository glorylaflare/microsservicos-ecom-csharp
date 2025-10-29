namespace BuildingBlocks.Messaging;

public interface IEventBus
{
    Task StartAsync(CancellationToken cancellationToken);
    Task PublishAsync<T>(T @event) where T : IntegrationEvent;
    Task SubscribeAsync<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
