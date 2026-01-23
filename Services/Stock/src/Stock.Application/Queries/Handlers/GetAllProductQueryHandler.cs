using FluentResults;
using MediatR;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Application.Responses;
namespace Stock.Application.Queries.Handlers;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<GetProductResponse>>>
{
    private readonly IProductReadService _productService;
    private readonly ILogger _logger;
    public GetAllProductQueryHandler(IProductReadService productService)
    {
        _productService = productService;
        _logger = Log.ForContext<GetAllProductQueryHandler>();
    }
    public async Task<Result<IEnumerable<GetProductResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName}", nameof(GetAllProductsQuery));
            var products = await _productService.GetAllAsync();
            if (products is null || !products.Any())
            {
                _logger.Warning("[WARN] No products found in the repository");
                return Result.Fail<IEnumerable<GetProductResponse>>("No products found.");
            }
            var response = products.Select(product => new GetProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CreatedAt,
                product.UpdatedAt
            ));
            _logger.Information("[INFO] Successfully fetched and mapped {ProductCount} products", response.Count());
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while fetching all products");
            return Result.Fail<IEnumerable<GetProductResponse>>("An error occurred while processing your request.");
        }
    }
}