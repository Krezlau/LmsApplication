using LmsApplication.CourseBoardModule.Data;
using LmsApplication.CourseBoardModule.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseBoardModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseBoardModuleApi(this IServiceCollection services, IConfiguration config)
    {
        services.AddCourseBoardModuleData(config);
        services.AddCourseBoardModuleServices();

        return services;
    }
}