using FluentResults;
using MediatR;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Application.Responses;
using Stock.Application.Specifications;

namespace Stock.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    private readonly IProductReadService _productService;
    private readonly ILogger _logger;

    public GetProductByIdQueryHandler(IProductReadService productService)
    {
        _productService = productService;
        _logger = Log.ForContext<GetProductByIdQueryHandler>();
    }

    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName} for Product ID {ProductId}", nameof(GetProductByIdQuery), request.Id);
            
            var product = await _productService.FindOneAsync(new ProductByIdSpec(request.Id));
            if (product is null)
            {
                _logger.Warning("[WARN] Product with ID {ProductId} not found", request.Id);
                return Result.Fail($"Product with ID {request.Id} not found.");
            }

            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CreatedAt,
                product.UpdatedAt
            );

            _logger.Information("[INFO] Product with ID {ProductId} retrieved successfully", request.Id);
            
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while retrieving product with ID {ProductId}", request.Id);
            return Result.Fail("An error occurred while processing the request.");
        }
    }
}