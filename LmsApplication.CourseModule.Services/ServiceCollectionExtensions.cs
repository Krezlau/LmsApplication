using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Courses.Validation;
using LmsApplication.CourseModule.Services.Courses;
using LmsApplication.CourseModule.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleServices(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseEditionService, CourseEditionService>();
        services.AddScoped<ICourseEditionSettingsService, CourseEditionSettingsService>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<CategoryPostModel>, ValidationService<CategoryPostModel>>();
        services.AddScoped<IValidationService<CoursePostModel>, ValidationService<CoursePostModel>>();
        services.AddScoped<IValidationService<CreateCourseEditionValidationModel>, ValidationService<CreateCourseEditionValidationModel>>();
        services.AddScoped<IValidationService<CourseEditionAddUserValidationModel>, ValidationService<CourseEditionAddUserValidationModel>>();
        services.AddScoped<IValidationService<CourseEditionRegisterModel>, ValidationService<CourseEditionRegisterModel>>();
        services.AddScoped<IValidationService<CourseEditionRemoveUserValidationModel>, ValidationService<CourseEditionRemoveUserValidationModel>>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseEditionRepository, CourseEditionRepository>();
        services.AddScoped<ICourseEditionSettingsRepository, CourseEditionSettingsRepository>();
        
        return services;
    }
    
    public static IServiceCollection AddCourseModuleRepositoriesAndData(this IServiceCollection services, IConfiguration config)
    {
        services.AddCourseModuleData(config);
        
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseEditionRepository, CourseEditionRepository>();
        services.AddScoped<ICourseEditionSettingsRepository, CourseEditionSettingsRepository>();
        
        return services;
    }
}