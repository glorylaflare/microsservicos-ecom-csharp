using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Order.Application.Integrations.Contexts;
using Order.Application.Interfaces;
using Serilog;

namespace Order.Application.Integrations;

public class OrderEmailPublisher : IOrderEmailPublisher
{
    private readonly IEventBus _eventBus;
    private readonly IUserReadService _userService;
    private readonly ILogger _logger;

    public OrderEmailPublisher(IEventBus eventBus, IUserReadService userService)
    {
        _eventBus = eventBus;
        _userService = userService;
        _logger = Log.ForContext<OrderEmailPublisher>();
    }

    public async Task PublishCompleted(Domain.Models.Order order)
    {
        _logger.Information("[INFO] Payment confirmed for OrderId: {OrderId}. Publishing completed order email request.", order.Id);

        var context = await BuildContext(order);
        if (context is null) return;

        var data = OrderEmailRequestData.Completed(
            context.Email,
            order.Id,
            context.Items
        );

        await Publish(data);
    }

    public async Task PublishFailed(Domain.Models.Order order)
    {
        _logger.Warning("[WARN] Payment failed for OrderId: {OrderId}. Publishing failed order email request.", order.Id);

        var context = await BuildContext(order);
        if (context is null) return;

        var data = OrderEmailRequestData.Failed(
            context.Email,
            order.Id,
            context.Items,
            "EXPIRADO"
        );

        await Publish(data);
    }

    public async Task PublishPending(Domain.Models.Order order, string checkoutUrl)
    {
        _logger.Information("[INFO] Payment pending for OrderId: {OrderId}. Publishing pending order email request.", order.Id);

        var context = await BuildContext(order);
        if (context is null) return;

        var data = OrderEmailRequestData.Pending(
            context.Email,
            order.Id,
            context.Items,
            checkoutUrl
        );

        await Publish(data);
    }

    private async Task<OrderEmailContext?> BuildContext(Domain.Models.Order order)
    {
        var user = await _userService.GetByIdAsync(order.UserId);

        if (user is null)
        {
            _logger.Warning("User not found for Id: {UserId}", order.UserId);
            return null;
        }

        var items = order.Items
            .Select(i => new OrderItemDto(i.ProductId, i.Quantity))
            .ToList();

        return new OrderEmailContext { 
            Email = user.Email, 
            Items = items 
        };
    }

    private Task Publish(OrderEmailRequestData data)
    {
        return _eventBus.PublishAsync(new OrderEmailRequestEvent(data));
    }
}
