using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using MediatR;
using Order.Application.Specifications;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Commands.StockRejected;

public class StockRejectedCommandHandler : IRequestHandler<StockRejectedCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public StockRejectedCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
        _logger = Log.ForContext<StockRejectedCommandHandler>();
    }

    public async Task<Unit> Handle(StockRejectedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName} for OrderId {OrderId}, Reason: {Reason}", nameof(StockRejectedCommand), request.OrderId, request.Reason);

        var order = await _orderRepository.FindOneAsync(new OrderByIdSpec(request.OrderId));
        if (order is null)
        {
            _logger.Warning("[WARN] Order with ID {OrderId} not found", request.OrderId);
            return Unit.Value;
        }

        order.Cancelled();
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        _logger.Warning("[WARN] Payment not successful for OrderId: {OrderId}. Publishing OrderFailedEvent.", order.Id);

        var orderDto = order.Items
            .Select(i => new OrderItemDto(i.ProductId, i.Quantity))
            .ToList();

        var data = new OrderFailedData(
            order.Id,
            orderDto,
            "INSUFFICIENT STOCK"
        );
        var evt = new OrderFailedEvent(data);
        await _eventBus.PublishAsync(evt);

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

        _logger.Information("[INFO] Order with ID {OrderId} has been cancelled due to stock rejection", order.Id);
        return Unit.Value;
    }
}