using LmsApplication.Core.Shared.Config;
using LmsApplication.CourseModule.Data;
using LmsApplication.CourseModule.Services;
using LmsApplication.CourseModule.Services.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseModule.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseModuleApi<TUserProv>(this IServiceCollection services, IConfiguration config)
        where TUserProv : class, IUserProvider
    {
        services.AddCourseModuleServices();
        services.AddCourseModuleData(config);
        services.AddScoped<IUserProvider, TUserProv>();
        
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