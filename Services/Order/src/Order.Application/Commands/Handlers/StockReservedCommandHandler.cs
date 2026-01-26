using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using MediatR;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Commands.Handlers;

public class StockReservedCommandHandler : IRequestHandler<StockReservedCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public StockReservedCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
        _logger = Log.ForContext<StockReservedCommandHandler>();
    }

    public async Task<Unit> Handle(StockReservedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName} for OrderId: {OrderId}", nameof(StockReservedCommand), request.OrderId);

        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order is null)
        {
            _logger.Warning("[WARN] Order with ID {OrderId} not found", request.OrderId);
            return Unit.Value;
        }

        order.SetTotalAmount(request.TotalAmount);
        order.Confirmed();

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        #region MongoDb update view
        _logger.Information("[INFO] OrderReadModel with ID {OrderId} status updated to {Status}", order.Id, order.Status);

        var viewModel = new OrderUpdatedData(
            order.Id,
            order.Status.ToString(),
            order.TotalAmount,
            order.UpdatedAt
        );
        var mongoEvt = new OrderUpdatedEvent(viewModel);
        await _eventBus.PublishAsync(mongoEvt);
        #endregion

        _logger.Information("[INFO] Order with ID {OrderId} has been confirmed with total amount {TotalAmount}", order.Id, request.TotalAmount);

        return Unit.Value;
    }
}