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
        _logger.Information("[INFO] Handling {EventName} for product: {ProductName}", nameof(CreateProductCommand), request.Name);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(CreateProductCommand), errors);
            return Result.Fail(errors);
        }
        try
        {
            var product = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity
            );
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            _logger.Information("[INFO] Product created successfully with ID: {ProductId}", product.Id);
            return Result.Ok(product.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while creating product {ProductName}", request.Name);
            return Result.Fail(new Error("An error occurred while creating the product."));
        }
    }
}