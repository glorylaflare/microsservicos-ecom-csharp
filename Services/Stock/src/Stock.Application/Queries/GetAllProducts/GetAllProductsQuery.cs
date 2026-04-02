using BuildingBlocks.SharedKernel.Common;
using FluentResults;
using MediatR;
using Stock.Application.Responses;

namespace Stock.Application.Queries.GetAllProducts;

public record GetAllProductsQuery(int Skip, int Take) : IRequest<Result<PageResult<ProductResponse>>>;