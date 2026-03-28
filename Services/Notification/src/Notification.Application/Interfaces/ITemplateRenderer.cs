namespace Notification.Application.Interfaces;

public interface ITemplateRenderer
{
    Task<string> RenderAsync<T>(Dictionary<string, string> data);
}
