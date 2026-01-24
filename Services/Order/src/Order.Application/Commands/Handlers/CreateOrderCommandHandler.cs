using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using FluentResults;
using FluentValidation;
using MediatR;
using Order.Domain.Interfaces;
using Order.Domain.Models;
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
        _logger.Information("[INFO] Handling {EventName} for {ItemsCount} items", nameof(CreateOrderCommand), request.Items.Count);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(CreateOrderCommand), errors);
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

            #region MongoDb view event
            _logger.Information("[INFO] Publishing events for Order {OrderId} for MongoDb", order.Id);

            var viewModel = new OrderCreatedData(
                order.Id,
                orderDto, 
                order.Status.ToString(), 
                order.CreatedAt
            );
            var mongoEvt = new OrderCreatedEvent(viewModel);
            await _eventBus.PublishAsync(mongoEvt);
            #endregion

            _logger.Information("[INFO] Published OrderCreatedEvent for Order {OrderId}", order.Id);

            var data = new OrderRequestedData(order.Id, orderDto);
            var evt = new OrderRequestedEvent(data);
            await _eventBus.PublishAsync(evt);

            _logger.Information("[INFO] Order {OrderId} created successfully with {ItemsCount} items", order.Id, request.Items.Count);
            return Result.Ok(order.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An error occurred while creating the order for the items {Items}", request.Items);
            return Result.Fail("An error occurred while creating the order");
        }
    }
}