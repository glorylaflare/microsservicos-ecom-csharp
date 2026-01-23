namespace Stock.Application.Responses;

public record GetProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    DateTime CreateAt,
    DateTime? UpdatedAt
);