using FluentResults;
using MediatR;
using Stock.Application.Responses;
namespace Stock.Application.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse>>;