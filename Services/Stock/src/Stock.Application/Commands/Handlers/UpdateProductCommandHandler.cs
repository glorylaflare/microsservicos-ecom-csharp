using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
using Stock.Domain.Interfaces;

namespace Stock.Application.Commands.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<UpdateProductCommand> _validator;
    private readonly ILogger _logger;

    public UpdateProductCommandHandler(IProductRepository productRepository, IValidator<UpdateProductCommand> validator)
    {
        _productRepository = productRepository;
        _validator = validator;
        _logger = Log.ForContext<UpdateProductCommandHandler>();
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {EventName} for product ID: {ProductId}", nameof(UpdateProductCommand), request.ProductId);

        _logger.Debug("Validating UpdateProductCommand: {Request}", request);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("Validation failed for UpdateProductCommand: {Errors}", errors);
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

            _logger.Debug("Updating product ID {ProductId} with new values", request.ProductId);
            product.UpdateProduct(
                request.Name ?? product.Name,
                request.Description ?? product.Description,
                request.Price);

            await _productRepository.SaveChangesAsync();

            _logger.Information("Product ID {ProductId} updated successfully", request.ProductId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating product ID {ProductId}", request.ProductId);
            return Result.Fail("An error occurred while updating the product.");
        }
    }
}
