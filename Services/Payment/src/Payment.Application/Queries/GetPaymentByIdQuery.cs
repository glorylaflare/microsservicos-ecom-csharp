using FluentResults;
using MediatR;
using Payment.Application.Responses;
namespace Payment.Application.Queries;

public record GetPaymentByIdQuery(int id) : IRequest<Result<GetPaymentResponse>>;