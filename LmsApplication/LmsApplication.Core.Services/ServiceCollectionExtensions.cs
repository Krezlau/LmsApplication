using LmsApplication.Core.Services.Graph;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IMicrosoftGraphServiceProvider, MicrosoftGraphServiceProvider>();
        services.AddScoped<IGraphService, GraphService>();
        
        return services;
    }
}