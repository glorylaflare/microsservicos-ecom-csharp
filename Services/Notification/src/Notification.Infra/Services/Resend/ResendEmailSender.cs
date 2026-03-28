using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;
using Notification.Infra.Configurations;
using Resend;
using Serilog;

namespace Notification.Infra.Services.Resend;

public class ResendEmailSender : IEmailSender
{
    private readonly IResend _resend;
    private readonly ResendSettings _settings;
    private readonly ILogger _logger;

    public ResendEmailSender(IResend resend, IOptions<ResendSettings> options)
    {
        _resend = resend;
        _settings = options.Value;
        _logger = Log.ForContext<ResendEmailSender>();
    }

    public async Task SendAsync(Message message)
    {
        _logger.Debug("[DEBUG] API KEY: {Key}", _settings.ApiKey);

        var emailMessage = new EmailMessage()
        {
            From = _settings.FromEmail,
            To = message.To,
            Subject = message.Subject,
            HtmlBody = message.Body,
        };

        _logger.Information("[INFO] Sending email.");

        var response = await _resend.EmailSendAsync(emailMessage);

        if (!response.Success)
        {
            var errorMessage = response.Exception?.Message ?? "Unknown error";

            _logger.Error("[ERROR] Error sending email via Resend. Error: {Error}", errorMessage);

            throw new EmailSendException(
                $"Error sending email via Resend: {errorMessage}"
            );
        }

        _logger.Information("[INFO] Email sent successfully.");
    }
}