using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;

namespace Stock.Application.Commands.Handlers;

public class OrderRequestCommandHandler : IRequestHandler<OrderRequestCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public OrderRequestCommandHandler(IProductRepository productRepository, IDbTransactionManager dbTransactionManager, IEventBus eventBus, ILogger logger)
    {
        _productRepository = productRepository;
        _dbTransactionManager = dbTransactionManager;
        _eventBus = eventBus;
        _logger = Log.ForContext<OrderRequestCommandHandler>();
    }

    public async Task<Unit> Handle(OrderRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var totalAmount = 0m;
            var reservedItems = new List<ProductItemDto>();

            await _dbTransactionManager.ExecuteResilientTransactionAsync(async () =>
            {
                foreach (var item in request.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product is null)
                    {
                        throw new InvalidOperationException("Product not found.");
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient stock.");
                    }

                    product.DecreaseStock(item.Quantity);
                    totalAmount += item.Quantity * product.Price;
                    reservedItems.Add(new ProductItemDto(
                        ProductId: item.ProductId,
                        Name: product.Name,
                        Description: product.Description,
                        Quantity: item.Quantity,
                        UnitPrice: product.Price
                    ));
                    _productRepository.Update(product);
                }
                await _productRepository.SaveChangesAsync();
            });

            await _eventBus.PublishAsync(new StockReservationResultEvent(
                orderId: request.OrderId,
                isReserved: true,
                items: reservedItems,
                totalAmount: totalAmount
            ));

            _logger.Information("{EventName} for Order ID: {OrderId} handled successfully", nameof(OrderRequestedEvent), request.OrderId);

            return Unit.Value;
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning("Stock reservation failed for Order ID: {OrderId} due to: {Reason}", request.OrderId, ex.Message);

            await _eventBus.PublishAsync(new StockReservationResultEvent(
                orderId: request.OrderId, 
                isReserved: false,
                reason: ex.Message
            ));
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for Order ID: {OrderId}", nameof(OrderRequestedEvent), request.OrderId);
            throw;
        }
    }
}
