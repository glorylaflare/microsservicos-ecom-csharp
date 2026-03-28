using Notification.Application.Interfaces;

namespace Notification.Infra.Services;

public class TemplateRenderer : ITemplateRenderer
{
    private readonly string _basePath = Path.Combine(AppContext.BaseDirectory, "Templates");

    public async Task<string> RenderAsync<T>(Dictionary<string, string> data)
    {
        var templateName = typeof(T).Name
            .Replace("EmailCommand", "") + ".html";

        var path = Path.Combine(_basePath, templateName);

        var content = await File.ReadAllTextAsync(path);

        foreach (var key in data)
        {
            content = content.Replace($"{{{{{key.Key}}}}}", key.Value);
        }

        return content;
    }
}
