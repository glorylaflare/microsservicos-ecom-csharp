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
        _logger.Information("[INFO] Handling {EventName} for product ID: {ProductId}", nameof(UpdateStockCommand), request.ProductId);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(UpdateStockCommand), errors);
            return Result.Fail(errors);
        }

        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                _logger.Warning("[WARN] Product with ID {ProductId} not found", request.ProductId);
                return Result.Fail($"Product with ID {request.ProductId} not found.");
            }

            product.DecreaseStock(request.Quantity);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            _logger.Information("[INFO] Stock for product ID {ProductId} updated successfully", request.ProductId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while updating stock for product ID {ProductId}", request.ProductId);
            return Result.Fail("An error occurred while updating the stock.");
        }
    }
}
