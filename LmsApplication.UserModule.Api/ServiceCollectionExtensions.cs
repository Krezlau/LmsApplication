using LmsApplication.UserModule.Data;
using LmsApplication.UserModule.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUserModuleData(configuration);
        services.AddUserModuleServices();

        return services;
    }
}