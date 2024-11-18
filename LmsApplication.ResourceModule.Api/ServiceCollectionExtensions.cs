using LmsApplication.ResourceModule.Data;
using LmsApplication.ResourceModule.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.ResourceModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResourceModuleApi(this IServiceCollection services, IConfiguration config)
    {
        services.AddResourceModuleServices();
        services.AddResourceModuleData(config);
        
        return services;
    }
}