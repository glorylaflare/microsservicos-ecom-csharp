using FluentResults;
using MediatR;
using Payment.Application.Responses;

namespace Payment.Application.Queries.GetAllPayments;

public record GetAllPaymentsQuery() : IRequest<Result<IEnumerable<GetPaymentResponse>>>;