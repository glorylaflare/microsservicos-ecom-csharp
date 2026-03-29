using BuildingBlocks.SharedKernel.Common;
using FluentResults;
using MediatR;
using Payment.Application.Responses;

namespace Payment.Application.Queries.GetAllPayments;

public record GetAllPaymentsQuery(int Skip, int Take) : IRequest<Result<PageResult<PaymentResponse>>>;