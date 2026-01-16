using FluentResults;
using MediatR;

namespace Stock.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<Result<int>>;
