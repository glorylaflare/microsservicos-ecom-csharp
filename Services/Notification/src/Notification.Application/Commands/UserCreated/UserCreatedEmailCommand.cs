using MediatR;

namespace Notification.Application.Commands.UserCreated;

public record UserCreatedEmailCommand(string Email, string Username) : IRequest<Unit>;