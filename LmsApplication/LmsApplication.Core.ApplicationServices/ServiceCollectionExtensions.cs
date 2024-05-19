using LmsApplication.Core.ApplicationServices.Courses;
using LmsApplication.Core.ApplicationServices.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.ApplicationServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<ICourseAppService, CourseAppService>();
        
        return services;
    }
}