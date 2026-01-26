using BuildingBlocks.Security.Context;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Security.Extensions
{
    public static class UserContextService
    {
        public static IServiceCollection AddUserContext(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
            return services;
        }
    }
}
