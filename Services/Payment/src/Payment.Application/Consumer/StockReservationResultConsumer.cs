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
        _logger.Information("Handling StockReservationResultEvent: {EventId}, OrderId: {OrderId}",
            @event.EventId, @event.Data.OrderId);

        try
        {
            if (@event.Data.IsReserved)
            {
                await _mediator.Send(new CreatePaymentCommand(
                    EventId: @event.EventId, 
                    Items: @event.Data.Items!));
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error handling StockReservationResultEvent: {EventId}, OrderId: {OrderId}, Error: {Error}",
                @event.EventId, @event.Data.OrderId, ex.Message);
            throw;
        }
    }
}
