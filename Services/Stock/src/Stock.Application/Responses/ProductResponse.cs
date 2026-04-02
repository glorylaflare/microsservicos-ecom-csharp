namespace Stock.Application.Responses;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    DateTime CreateAt,
    DateTime? UpdatedAt
);