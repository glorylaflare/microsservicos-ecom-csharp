using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using MediatR;
using Order.Application.Interfaces;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Commands.Handlers;

public class PaymentUpdatedCommandHandler : IRequestHandler<PaymentUpdatedCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserReadService _userService; 
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public PaymentUpdatedCommandHandler(IOrderRepository orderRepository, IEventBus eventBus, IUserReadService userService)
    {
        _orderRepository = orderRepository;
        _userService = userService;
        _eventBus = eventBus;
        _logger = Log.ForContext<PaymentUpdatedCommandHandler>();
    }

    public async Task<Unit> Handle(PaymentUpdatedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Received payment update for OrderId: {OrderId} with Status: {Status}", request.OrderId, request.Status);

        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                _logger.Warning("[WARN] Order with Id: {OrderId} not found for payment update", request.OrderId);
                return Unit.Value;
            }

            switch (request.Status.ToLower())
            {
                case "paid":
                    _logger.Information("[INFO] Payment successful for OrderId: {OrderId}. Updating order status to Completed.", request.OrderId);
                    await ConfirmedPayment(order);
                    break;
                case "failed":
                    _logger.Information("[INFO] Payment failed for OrderId: {OrderId}. Updating order status to Cancelled.", request.OrderId);
                    await FailedPayment(order);
                    break;
                default:
                    _logger.Information("[INFO] Payment pending for OrderId: {OrderId}. No order status change.", request.OrderId);
                    break;
            }

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error while handling {EventName} for Order ID: {OrderId}", nameof(PaymentUpdatedCommandHandler), request.OrderId);
            throw;
        }
    }

    public async Task ConfirmedPayment(Domain.Models.Order order)
    {
        order.Completed();
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        await PublishEvent(
            order: order, 
            isFailed: false
        );
    }

    public async Task FailedPayment(Domain.Models.Order order)
    {
        order.Cancelled();
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        await PublishEvent(
            order: order,
            isFailed: true
        );
    }

    public async Task PublishEvent(Domain.Models.Order order, bool isFailed)
    {
        #region MongoDb updated view
        _logger.Information("[INFO] Publishing OrderUpdatedEvent for OrderId: {OrderId} with Status: {Status}", order.Id, order.Status);

        var data = new OrderUpdatedData(
            order.Id,
            order.Status.ToString(),
            order.TotalAmount,
            order.UpdatedAt
        );
        var evt = new OrderUpdatedEvent(data);
        await _eventBus.PublishAsync(evt);
        #endregion

        if (isFailed)
        {
            var user = await _userService.GetByIdAsync(order.UserId);

            _logger.Warning("[WARN] Payment not successful for OrderId: {OrderId}. Publishing OrderFailedEvent.", order.Id);

            var reason = "EXPIRADO";

            var orderDto = order.Items
                .Select(i => new OrderItemDto(i.ProductId, i.Quantity))
                .ToList();

            var dataFailed = new OrderFailedData(
                orderDto,
                reason,
                user!.Email
            );
            var evtFailed = new OrderFailedEvent(dataFailed);
            await _eventBus.PublishAsync(evtFailed);
        }
    }
}
