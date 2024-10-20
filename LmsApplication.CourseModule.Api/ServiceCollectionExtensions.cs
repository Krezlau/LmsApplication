using LmsApplication.CourseModule.Data;
using LmsApplication.CourseModule.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleApi(this IServiceCollection services, IConfiguration config)
    {
        services.AddCourseModuleServices();
        services.AddCourseModuleData(config);
        
        return services;
    }
}