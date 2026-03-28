using FluentResults;
using MediatR;
using Order.Domain.Models;
namespace Order.Application.Commands.CreateOrder;

public record CreateOrderCommand(List<OrderItem> Items) : IRequest<Result<int>>;