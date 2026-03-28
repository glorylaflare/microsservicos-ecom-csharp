using FluentResults;
using MediatR;
using Order.Application.Responses;

namespace Order.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<Result<GetOrderComposeResponse>>;