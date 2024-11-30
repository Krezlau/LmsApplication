using LmsApplication.CourseBoardModule.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseBoardModule.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseBoardModuleData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CourseBoardDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("CourseBoardDb")));

        return services;
    }
}