using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Domain.Exceptions;
using Stock.Domain.Interfaces;
namespace Stock.Application.Commands.Handlers;

public class OrderRequestCommandHandler : IRequestHandler<OrderRequestCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;
    public OrderRequestCommandHandler(IProductRepository productRepository, IDbTransactionManager dbTransactionManager, IEventBus eventBus)
    {
        _productRepository = productRepository;
        _dbTransactionManager = dbTransactionManager;
        _eventBus = eventBus;
        _logger = Log.ForContext<OrderRequestCommandHandler>();
    }
    public async Task<Unit> Handle(OrderRequestCommand request, CancellationToken cancellationToken)
    {
        var totalAmount = 0m;
        var reservedItems = new List<ProductItemDto>();
        try
        {
            await _dbTransactionManager.ExecuteResilientTransactionAsync(async () =>
            {
                foreach (var item in request.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product is null)
                    {
                        throw new ProductNotFoundException(item.ProductId);
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InsufficientStockException(item.ProductId, item.Quantity, product.StockQuantity);
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
            var data = StockReservationResultData.Success(
                orderId: request.OrderId,
                items: reservedItems,
                totalAmount: totalAmount
            );
            var evt = new StockReservationResultEvent(data);
            await _eventBus.PublishAsync(evt);
            _logger.Information("[INFO] {EventName} for Order ID: {OrderId} handled successfully", nameof(OrderRequestedEvent), request.OrderId);
        }
        catch (DomainException ex)
        {
            _logger.Warning("[WARN] Stock reservation failed for Order ID: {OrderId} due to: {Reason}", request.OrderId, ex.Message);
            var data = StockReservationResultData.Failure(
                orderId: request.OrderId,
                reason: ex.Message
            );
            var evt = new StockReservationResultEvent(data);
            await _eventBus.PublishAsync(evt);
        }
        _logger.Information("[INFO] {EventName} for Order ID: {OrderId} processing completed", nameof(OrderRequestedEvent), request.OrderId);
        return Unit.Value;
    }
}