using FluentResults;
using MediatR;
using Stock.Application.Responses;
namespace Stock.Application.Queries;

public record GetAllProductsQuery() : IRequest<Result<IEnumerable<GetProductResponse>>>;