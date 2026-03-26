namespace Notification.Domain.Exceptions;

public class EmailSendException : Exception
{
    public EmailSendException(string message) 
        : base(message) { }
}
