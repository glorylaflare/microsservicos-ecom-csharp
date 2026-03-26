using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.Handlers;

public class OrderCompletedEmailCommandHandler : IRequestHandler<OrderCompletedEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;

    public OrderCompletedEmailCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _logger = Log.ForContext<OrderCompletedEmailCommandHandler>();
    }

    public async Task<Unit> Handle(OrderCompletedEmailCommand request, CancellationToken ct)
    {
        try
        {
            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pedido concluído",
                Body = "Seu pedido foi concluído com sucesso!"
            });

            return Unit.Value;
        }
        catch (EmailSendException ex)
        {
            _logger.Error(ex, "[ERROR] Erro ao enviar e-mail");
            throw; 
        }
    }
}
