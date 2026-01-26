using BuildingBlocks.Messaging;

namespace User.Api.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    }
}
