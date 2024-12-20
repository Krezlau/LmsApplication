using LmsApplication.CourseBoardModule.Data;
using LmsApplication.CourseBoardModule.Services;
using LmsApplication.CourseBoardModule.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseBoardModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseBoardModuleApi<TUserProv, TCourseEditionProv>(this IServiceCollection services, IConfiguration config)
        where TUserProv : class, IUserProvider
        where TCourseEditionProv: class, ICourseEditionProvider
    {
        services.AddCourseBoardModuleData(config);
        services.AddCourseBoardModuleServices(config);
        
        services.AddScoped<IUserProvider, TUserProv>();
        services.AddScoped<ICourseEditionProvider, TCourseEditionProv>();

        return services;
    }
}