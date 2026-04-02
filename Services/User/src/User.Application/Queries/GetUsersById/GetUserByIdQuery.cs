using FluentResults;
using MediatR;
using User.Application.Responses;

namespace User.Application.Queries.GetUsersById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserResponse>>;