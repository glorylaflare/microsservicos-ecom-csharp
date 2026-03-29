using MediatR;
using Serilog;
using Stock.Application.Commands.CreateProduct;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;

namespace Stock.Application.Commands.OrderFailed;

public class OrderFailedCommandHandler : IRequestHandler<OrderFailedCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly ILogger _logger;

    public OrderFailedCommandHandler(IProductRepository productRepository, IDbTransactionManager dbTransactionManager)
    {
        _productRepository = productRepository;
        _dbTransactionManager = dbTransactionManager;
        _logger = Log.ForContext<OrderFailedCommandHandler>();
    }

    public async Task<Unit> Handle(OrderFailedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName}", nameof(OrderFailedCommand));

        try
        {
            await _dbTransactionManager.ExecuteResilientTransactionAsync(async () =>
            {
                foreach (var items in request.Items)
                {
                    _logger.Information("[INFO] Restoring stock for Product ID {ProductId} with Quantity {Quantity}", items.ProductId, items.Quantity);

                    var product = await _productRepository.GetByIdAsync(items.ProductId);

                    if (product == null)
                    {
                        _logger.Warning("[WARN] Product with ID {ProductId} not found while handling {CommandName}", items.ProductId, nameof(OrderFailedCommandHandler));
                        continue;
                    }

                    product.Restock(items.Quantity);
                    _productRepository.Update(product);

                    _logger.Information("[INFO] Restored stock for Product ID {ProductId}. New Stock Quantity: {StockQuantity}", items.ProductId, product.StockQuantity);
                }

                await _productRepository.SaveChangesAsync();
            });

            _logger.Information("[INFO] Successfully handled {CommandName}", nameof(OrderFailedCommandHandler));

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An error occurred while handling {CommandName}", nameof(OrderFailedCommandHandler));
            throw;
        }

    }
}
