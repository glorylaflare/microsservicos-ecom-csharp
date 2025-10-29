using FluentResults;
using MediatR;
using Order.Application.Responses;

namespace Order.Application.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<Result<GetOrderResponse>>;