using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services;
    }
}