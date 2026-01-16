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
        _logger.Information("[INFO] Handling {EventName} for product ID: {ProductId}", nameof(UpdateProductCommand), request.ProductId);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(UpdateProductCommand), errors);
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
            product.UpdateProduct(
                request.Name ?? product.Name,
                request.Description ?? product.Description,
                request.Price);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            _logger.Information("[INFO] Product ID {ProductId} updated successfully", request.ProductId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while updating product ID {ProductId}", request.ProductId);
            return Result.Fail("An error occurred while updating the product.");
        }
    }
}
