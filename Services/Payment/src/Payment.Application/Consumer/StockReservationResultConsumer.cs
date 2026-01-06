using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;

namespace Payment.Application.Consumer;

public class StockReservationResultConsumer : IIntegrationEventHandler<StockReservationResultEvent>
{
    public Task HandleAsync(StockReservationResultEvent @event)
    {
        throw new NotImplementedException();
    }
}
