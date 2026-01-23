using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Order.Application.Commands;
using Serilog;

namespace Order.Application.Consumers;

public class StockReservationResultConsumer : IIntegrationEventHandler<StockReservationResultEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public StockReservationResultConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<StockReservationResultConsumer>();
    }

    public async Task HandleAsync(StockReservationResultEvent @event)
    {
        _logger.Information("[INFO] Handling {EventName} for OrderId: {OrderId}, IsReserved: {IsReserved}", nameof(StockReservationResultEvent), @event.Data.OrderId, @event.Data.IsReserved);

        if (!@event.Data.IsReserved)
        {
            await _mediator.Send(new StockRejectedCommand(@event.Data.OrderId, @event.Data.Reason));
            return;
        }

        await _mediator.Send(new StockReservedCommand(@event.Data.OrderId, @event.Data.TotalAmount));
    }
}
