using LmsApplication.Core.Services.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProviderService, TenantProviderService>();
        return services;
    }
}