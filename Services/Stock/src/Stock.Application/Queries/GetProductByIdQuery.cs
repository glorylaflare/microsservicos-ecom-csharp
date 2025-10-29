using FluentResults;
using MediatR;
using Stock.Application.Responses;

namespace Stock.Application.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Result<GetProductResponse>>;
