using LmsApplication.Core.ApplicationServices.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.ApplicationServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthAppService, AuthAppService>();
        
        return services;
    }
}