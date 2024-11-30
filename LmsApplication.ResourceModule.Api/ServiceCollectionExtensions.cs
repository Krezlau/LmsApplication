using LmsApplication.ResourceModule.Data;
using LmsApplication.ResourceModule.Services;
using LmsApplication.ResourceModule.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.ResourceModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResourceModuleApi<TUserProv, TCourseProv, TCourseEditionProv>(
        this IServiceCollection services, IConfiguration config)
        where TUserProv : class, IUserProvider
        where TCourseProv : class, ICourseProvider
        where TCourseEditionProv: class, ICourseEditionProvider
    {
        services.AddResourceModuleServices(config);
        services.AddResourceModuleData(config);
        
        services.AddScoped<IUserProvider, TUserProv>();
        services.AddScoped<ICourseProvider, TCourseProv>();
        services.AddScoped<ICourseEditionProvider, TCourseEditionProv>();
        
        return services;
    }
}