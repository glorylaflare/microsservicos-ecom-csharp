using FluentResults;
using MediatR;
using Serilog;
using Stock.Application.Responses;
using Stock.Domain.Interfaces;

namespace Stock.Application.Queries.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger _logger;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _logger = Log.ForContext<GetProductByIdQueryHandler>();
    }

    public async Task<Result<GetProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Handling {EventName} for Product ID {ProductId}", nameof(GetProductByIdQuery), request.Id);

            _logger.Debug("Retrieving product with ID {ProductId} from repository", request.Id);
            var product = await _productRepository.GetProductByIdAsync(request.Id);
            if (product is null)
            {
                _logger.Warning("Product with ID {ProductId} not found", request.Id);
                return Result.Fail<GetProductResponse>($"Product with ID {request.Id} not found.");
            }

            var response = new GetProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CreatedAt,
                product.UpdatedAt
            );

            _logger.Information("Product with ID {ProductId} retrieved successfully", request.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while retrieving product with ID {ProductId}", request.Id);
            return Result.Fail<GetProductResponse>("An error occurred while processing the request.");
        }
    }
}
