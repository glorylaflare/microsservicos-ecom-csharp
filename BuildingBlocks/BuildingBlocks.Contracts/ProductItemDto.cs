namespace BuildingBlocks.Contracts;

public record ProductItemDto(
    int ProductId,
    string Name,
    string Description,
    int Quantity,
    decimal UnitPrice
);