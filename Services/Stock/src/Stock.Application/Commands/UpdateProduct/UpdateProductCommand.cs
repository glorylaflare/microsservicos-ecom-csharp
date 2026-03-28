using FluentResults;
using MediatR;

namespace Stock.Application.Commands.UpdateProduct;

public record UpdateProductCommand(
    int ProductId,
    string Name,
    string Description,
    decimal Price
) : IRequest<Result>;