using LmsApplication.Core.Data.Config;
using LmsApplication.CourseModule.Data;
using LmsApplication.CourseModule.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleApi(this IServiceCollection services, IConfiguration config)
    {
        services.AddCourseModuleServices();
        services.AddCourseModuleData(config);
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(AuthPolicies.AdminPolicy, policy => policy.RequireRole("Admin"));
            opt.AddPolicy(AuthPolicies.TeacherPolicy, policy => policy.RequireRole("Teacher"));
            opt.AddPolicy(AuthPolicies.StudentPolicy, policy => policy.RequireAuthenticatedUser());
            
            opt.DefaultPolicy = opt.GetPolicy(AuthPolicies.StudentPolicy)!;
        });
        return services;
    }
}