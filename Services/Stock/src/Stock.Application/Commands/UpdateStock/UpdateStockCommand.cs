using FluentResults;
using MediatR;

namespace Stock.Application.Commands.UpdateStock;

public record UpdateStockCommand(
    int ProductId,
    int Quantity
) : IRequest<Result>;