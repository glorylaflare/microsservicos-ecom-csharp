using Notification.Domain.Models;

namespace Notification.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(Message message);
}
