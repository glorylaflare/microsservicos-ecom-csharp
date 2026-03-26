using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.Handlers;

internal class OrderFailedEmailCommandHandler : IRequestHandler<OrderFailedEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;

    public OrderFailedEmailCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _logger = Log.ForContext<OrderFailedEmailCommand>();
    }

    public async Task<Unit> Handle(OrderFailedEmailCommand request, CancellationToken ct)
    {
        try
        {
            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pedido falhou",
                Body = "Seu pedido, infelizmente, não foi concluído!"
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
