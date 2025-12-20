using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentResults;
using FluentValidation;
using MediatR;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Commands.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IValidator<CreateOrderCommand> validator, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _validator = validator;
        _eventBus = eventBus;
        _logger = Log.ForContext<CreateOrderCommandHandler>();
    }

    public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {EventName} for {ItemsCount} items", nameof(CreateOrderCommand), request.Items.Count);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("Validation failed for CreateOrderCommand: {Errors}", errors);
            return Result.Fail(errors);
        }

        try
        {
            var order = new Domain.Models.Order(request.Items);
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            var orderDto = order.Items
                .Select(i => new OrderItemDto(i.ProductId, i.Quantity))
                .ToList();

            var evt = new OrderRequestedEvent(order.Id, orderDto);
            await _eventBus.PublishAsync(evt);

            _logger.Information("Order {OrderId} created successfully with {ItemsCount} items", order.Id, request.Items.Count);
            return Result.Ok(order.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while creating the order for the items {Items}", request.Items);
            return Result.Fail("An error occurred while creating the order");
        } 
    }
}
