using FluentResults;
using MediatR;
using Serilog;
using Stock.Application.Responses;
using Stock.Domain.Interfaces;

namespace Stock.Application.Queries.Handlers;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<GetProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger _logger;

    public GetAllProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _logger = Log.ForContext<GetAllProductQueryHandler>();
    }

    public async Task<Result<IEnumerable<GetProductResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Handling {EventName}", nameof(GetAllProductsQuery));

            _logger.Debug("Fetching all products from the repository");
            var products = await _productRepository.GetAllProductsAsync();
            if (products is null || !products.Any())
            {
                _logger.Warning("No products found in the repository");
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

            _logger.Information("Successfully fetched and mapped {ProductCount} products", response.Count());
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all products");
            return Result.Fail<IEnumerable<GetProductResponse>>("An error occurred while processing your request.");
        }
    }
}
