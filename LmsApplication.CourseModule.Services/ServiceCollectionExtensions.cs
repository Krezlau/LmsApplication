using FluentValidation;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Courses;
using LmsApplication.CourseModule.Services.Providers;
using LmsApplication.CourseModule.Services.Repositories;
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
        services.AddScoped<IValidationService<CourseEditionPostModel>, ValidationService<CourseEditionPostModel>>();
        services.AddScoped<IValidationService<CourseEditionAddUserModel>, ValidationService<CourseEditionAddUserModel>>();
        services.AddScoped<IValidationService<CourseEditionRegisterModel>, ValidationService<CourseEditionRegisterModel>>();
        services.AddScoped<IValidationService<CourseEditionRemoveUserModel>, ValidationService<CourseEditionRemoveUserModel>>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseEditionRepository, CourseEditionRepository>();
        services.AddScoped<ICourseEditionSettingsRepository, CourseEditionSettingsRepository>();

        services.AddScoped<ICourseEditionProvider, CourseEditionProvider>();
        services.AddScoped<ICourseProvider, CourseProvider>();
        
        return services;
    }
    
}