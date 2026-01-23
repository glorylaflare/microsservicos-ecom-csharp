using Auth.Api.Responses;
using FluentResults;
using MediatR;
namespace Auth.Api.Commands;

public record AuthenticateUserCommand(
    string Email,
    string Password
) : IRequest<Result<TokenResponse>>;