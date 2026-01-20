using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Payment.Application.Commands;
using Serilog;

namespace Payment.Application.Consumer;

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
        _logger.Information("[INFO] Handling {EventName}: {EventId}, OrderId: {OrderId}", nameof(StockReservationResultEvent), @event.EventId, @event.Data.OrderId);

        try
        {
            if (@event.Data.IsReserved)
            {
                await _mediator.Send(new CreatePaymentCommand(@event.EventId, @event.Data.OrderId, @event.Data.TotalAmount, @event.Data.Items!));
            }
        }
        catch (Exception ex)
        {
            _logger.Error("[ERROR] Error handling {EventName}: {EventId}, OrderId: {OrderId}, Error: {Error}",
                nameof(StockReservationResultEvent), @event.EventId, @event.Data.OrderId, ex.Message);
            throw;
        }
    }
}
