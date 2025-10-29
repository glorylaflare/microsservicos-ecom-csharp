using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
using Stock.Domain.Interfaces;

namespace Stock.Application.Commands.Handlers;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<UpdateStockCommand> _validator;
    private readonly ILogger _logger;

    public UpdateStockCommandHandler(IProductRepository productRepository, IValidator<UpdateStockCommand> validator)
    {
        _productRepository = productRepository;
        _validator = validator;
        _logger = Log.ForContext<UpdateStockCommandHandler>();
    }

    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {EventName} for product ID: {ProductId}", nameof(UpdateStockCommand), request.ProductId);

        _logger.Debug("Validating UpdateStockCommand: {Request}", request);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("Validation failed for UpdateStockCommand: {Errors}", errors);
            return Result.Fail(errors);
        }

        try
        {
            _logger.Debug("Retrieving product with ID: {ProductId}", request.ProductId);
            var product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product is null)
            {
                _logger.Warning("Product with ID {ProductId} not found", request.ProductId);
                return Result.Fail($"Product with ID {request.ProductId} not found.");
            }

            _logger.Debug("Decreasing stock for product ID {ProductId} by {Quantity}", request.ProductId, request.Quantity);
            product.DecreaseStock(request.Quantity);

            await _productRepository.SaveChangesAsync();

            _logger.Information("Stock for product ID {ProductId} updated successfully", request.ProductId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating stock for product ID {ProductId}", request.ProductId);
            return Result.Fail("An error occurred while updating the stock.");
        }
    }
}
