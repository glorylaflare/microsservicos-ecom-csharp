using FluentResults;
using MediatR;
namespace Stock.Application.Commands;

public record UpdateStockCommand(
    int ProductId,
    int Quantity
) : IRequest<Result>;