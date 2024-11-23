using LmsApplication.UserModule.Data;
using LmsApplication.UserModule.Services;
using LmsApplication.UserModule.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleApi<TCourseEditionProv>(this IServiceCollection services, IConfiguration configuration)
        where TCourseEditionProv : class, ICourseEditionProvider
    {
        services.AddUserModuleData(configuration);
        services.AddUserModuleServices();

        services.AddScoped<ICourseEditionProvider, TCourseEditionProv>();
        
        return services;
    }
}