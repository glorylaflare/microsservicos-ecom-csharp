using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.OrderPending;

public class OrderPendingEmailCommandHandler : IRequestHandler<OrderPendingEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger _logger;

    public OrderPendingEmailCommandHandler(IEmailSender emailSender, ITemplateRenderer templateRenderer)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _logger = Log.ForContext<OrderPendingEmailCommandHandler>();
    }

    public async Task<Unit> Handle(OrderPendingEmailCommand request, CancellationToken ct)
    {
        _logger.Information("[INFO] Handling {CommandName} for order {OrderId}",
            nameof(OrderPendingEmailCommand), request.OrderId);

        try
        {
            var html = await _templateRenderer.RenderAsync<OrderPendingEmailCommand>(
                new Dictionary<string, string>
                {
                    ["OrderId"] = request.OrderId.ToString(),
                    ["CheckoutUrl"] = request.CheckoutUrl.ToString()
                }
            );

            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pagamento pendente",
                Body = html
            });

            _logger.Information("[INFO] Order pending email sent successfully for order {OrderId}",
                request.OrderId);

            return Unit.Value;
        }
        catch (EmailSendException ex)
        {
            _logger.Error(ex, "[ERROR] Failed to send order pending email for order {OrderId}",
                request.OrderId);
            throw;
        }
    }
}
