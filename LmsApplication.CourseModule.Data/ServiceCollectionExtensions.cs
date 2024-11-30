using LmsApplication.CourseModule.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CourseDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("CourseDb")));

        return services;
    }
}