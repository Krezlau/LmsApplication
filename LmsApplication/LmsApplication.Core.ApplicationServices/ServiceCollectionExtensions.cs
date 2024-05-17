using LmsApplication.Core.ApplicationServices.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.ApplicationServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAppService, UserAppService>();
        
        return services;
    }
}