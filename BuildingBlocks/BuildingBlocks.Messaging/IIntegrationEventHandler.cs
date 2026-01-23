namespace BuildingBlocks.Messaging;

public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEventBase
{
    Task HandleAsync(TEvent @event);
}