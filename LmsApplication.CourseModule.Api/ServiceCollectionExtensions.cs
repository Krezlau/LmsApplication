using LmsApplication.CourseModule.Data;
using LmsApplication.CourseModule.Services;
using LmsApplication.CourseModule.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleApi<TUserProv>(this IServiceCollection services, IConfiguration config)
        where TUserProv : class, IUserProvider
    {
        services.AddCourseModuleServices();
        services.AddCourseModuleData(config);
        services.AddScoped<IUserProvider, TUserProv>();
        
        return services;
    }
}