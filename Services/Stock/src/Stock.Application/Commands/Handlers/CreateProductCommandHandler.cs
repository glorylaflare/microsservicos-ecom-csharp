using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;

namespace Stock.Application.Commands.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly ILogger _logger;

    public CreateProductCommandHandler(IProductRepository productRepository, IValidator<CreateProductCommand> validator)
    {
        _productRepository = productRepository;
        _validator = validator;
        _logger = Log.ForContext<CreateProductCommandHandler>();
    }

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {EventName} for product: {ProductName}", nameof(CreateProductCommand), request.Name);

        _logger.Debug("Validating CreateProductCommand: {Request}", request);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("Validation failed for CreateProductCommand: {Errors}", errors);
            return Result.Fail(errors);
        }

        try
        {
            _logger.Debug("Creating new product with name: {ProductName}", request.Name);
            var product = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity
            );

            _logger.Debug("Adding product to repository: {Product}", product);
            await _productRepository.AddProductAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.Information("Product created successfully with ID: {ProductId}", product.Id);
            return Result.Ok(product.Id);

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating product {ProductName}", request.Name);
            return Result.Fail(new Error("An error occurred while creating the product."));
        }
    }
}
