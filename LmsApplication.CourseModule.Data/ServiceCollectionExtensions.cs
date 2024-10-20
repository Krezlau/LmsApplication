using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CourseDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("course-db")));

        services.AddScoped<ICourseEditionService, CourseEditionService>();
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}