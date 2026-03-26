using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.Handlers;

public class OrderPendingEmailCommandHandler : IRequestHandler<OrderPendingEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;

    public OrderPendingEmailCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _logger = Log.ForContext<OrderPendingEmailCommand>();
    }

    public async Task<Unit> Handle(OrderPendingEmailCommand request, CancellationToken ct)
    {
        try
        {
            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pedido concluído",
                Body = $"Olá, seu pedido foi confirmado mas agora estamos aguardando seu pagamento, realize o pagamento através do link: {request.CheckoutUrl}!"
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
