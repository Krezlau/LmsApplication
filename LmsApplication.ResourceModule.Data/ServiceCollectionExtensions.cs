using LmsApplication.ResourceModule.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.ResourceModule.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResourceModuleData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ResourceDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("resource-db")));

        return services;
    }
}