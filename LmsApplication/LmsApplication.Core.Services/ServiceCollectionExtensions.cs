using LmsApplication.Core.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}