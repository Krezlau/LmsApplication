using LmsApplication.Core.Config.ConfigModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureModels(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TenantsModel>(config.GetSection(TenantsModel.Key));
        services.Configure<AppTenantsModel>(config.GetSection(AppTenantsModel.Key));
        
        return services;
    }
}