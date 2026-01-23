using FluentResults;
using MediatR;
using User.Application.Responses;
namespace User.Application.Queries;
public record GetUserByIdQuery(int Id) : IRequest<Result<GetUserResponse>>;