using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Serilog;

namespace Notification.Application.Commands.OrderCompleted;

public class OrderCompletedEmailCommandHandler : IRequestHandler<OrderCompletedEmailCommand, Unit>
{
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger _logger;

    public OrderCompletedEmailCommandHandler(IEmailSender emailSender, ITemplateRenderer templateRenderer)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _logger = Log.ForContext<OrderCompletedEmailCommandHandler>();
    }

    public async Task<Unit> Handle(OrderCompletedEmailCommand request, CancellationToken ct)
    {
        _logger.Information("[INFO] Handling {CommandName} for order {OrderId}",
            nameof(OrderCompletedEmailCommand), request.OrderId);

        try
        {
            var html = await _templateRenderer.RenderAsync<OrderCompletedEmailCommand>(
                new Dictionary<string, string>
                {
                    ["OrderId"] = request.OrderId.ToString(),
                    ["Items"] = string.Join("", request.Items.Select(i => $"<li>{i.ProductId} - {i.Quantity}</li>"))
                }
            );

            await _emailSender.SendAsync(new Message
            {
                To = request.Email,
                Subject = "Pedido concluído",
                Body = html
            });

            _logger.Information("[INFO] Order completed email sent successfully for order {OrderId}",
                request.OrderId);

            return Unit.Value;
        }
        catch (EmailSendException ex)
        {
            _logger.Error(ex, "[ERROR] Failed to send order completed email for order {OrderId}",
                request.OrderId);
            throw; 
        }
    }
}
