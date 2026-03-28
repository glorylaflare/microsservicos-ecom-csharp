using FluentResults;
using MediatR;
using User.Application.Responses;
namespace User.Application.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<Result<IEnumerable<GetUserResponse>>>;