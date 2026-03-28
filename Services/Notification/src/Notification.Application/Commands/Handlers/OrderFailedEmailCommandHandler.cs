using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.Handlers;

internal class OrderFailedEmailCommandHandler : IRequestHandler<OrderFailedEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger _logger;

    public OrderFailedEmailCommandHandler(IEmailSender emailSender, ITemplateRenderer templateRenderer)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _logger = Log.ForContext<OrderFailedEmailCommandHandler>();
    }

    public async Task<Unit> Handle(OrderFailedEmailCommand request, CancellationToken ct)
    {
        _logger.Information("[INFO] Handling {CommandName} for order {OrderId}",
            nameof(OrderFailedEmailCommand), request.OrderId);

        try
        {
            var html = await _templateRenderer.RenderAsync<OrderFailedEmailCommand>(
                new Dictionary<string, string>
                {
                    ["OrderId"] = request.OrderId.ToString(),
                    ["Reason"] = request.Reason.ToString()
                }
            );

            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pedido falhou",
                Body = html
            });

            _logger.Information("[INFO] Order failed email sent successfully for order {OrderId}",
                request.OrderId);

            return Unit.Value;
        }
        catch (EmailSendException ex)
        {
            _logger.Error(ex, "[ERROR] Failed to send order failed email for order {OrderId}",
                request.OrderId);
            throw;
        }
    }
}
