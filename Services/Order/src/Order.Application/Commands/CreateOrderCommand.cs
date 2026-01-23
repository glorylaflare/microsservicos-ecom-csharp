using FluentResults;
using MediatR;
using Order.Domain.Models;
namespace Order.Application.Commands;
public record CreateOrderCommand(List<OrderItem> Items) : IRequest<Result<int>>;