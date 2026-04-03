using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.UserCreated;

public class UserCreatedEmailCommandHandler : IRequestHandler<UserCreatedEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger _logger;

    public UserCreatedEmailCommandHandler(IEmailSender emailSender, ITemplateRenderer templateRenderer)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _logger = Log.ForContext<UserCreatedEmailCommandHandler>();
    }

    public async Task<Unit> Handle(UserCreatedEmailCommand request, CancellationToken ct)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(UserCreatedEmailCommand));

        try
        {
            var html = await _templateRenderer.RenderAsync<UserCreatedEmailCommand>(
                new Dictionary<string, string>
                {
                    ["Username"] = request.Username,
                    ["Email"] = request.Email,
                }
            );

            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Bem-vindo(a) ao nosso serviço de E-Commerce",
                Body = html
            });

            _logger.Information("[INFO] User created email sent successfully");

            return Unit.Value;
        }
        catch (EmailSendException ex)
        {
            _logger.Error(ex, "[ERROR] Failed to send user created email");
            throw;
        }
    }
}
