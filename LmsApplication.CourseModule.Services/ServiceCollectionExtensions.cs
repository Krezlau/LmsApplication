using FluentValidation;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.CourseModule.Services.Courses;
using LmsApplication.CourseModule.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleServices(this IServiceCollection services)
    {
        services.AddScoped<ICourseAppService, CourseAppService>();
        services.AddScoped<ICourseEditionAppService, CourseEditionAppService>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<CategoryPostModel>, ValidationService<CategoryPostModel>>();
        services.AddScoped<IValidationService<CoursePostModel>, ValidationService<CoursePostModel>>();
        services.AddScoped<IValidationService<CourseEditionPostModel>, ValidationService<CourseEditionPostModel>>();
        services.AddScoped<IValidationService<CourseEditionAddUserModel>, ValidationService<CourseEditionAddUserModel>>();
        
        return services;
    }
    
}