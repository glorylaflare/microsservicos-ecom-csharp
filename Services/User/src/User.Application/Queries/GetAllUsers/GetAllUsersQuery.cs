using BuildingBlocks.SharedKernel.Common;
using FluentResults;
using MediatR;
using User.Application.Responses;

namespace User.Application.Queries.GetAllUsers;

public record GetAllUsersQuery(int Skip, int Take) : IRequest<Result<PageResult<UserResponse>>>;