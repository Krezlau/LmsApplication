using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCoursesDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ITenantProviderService, TenantProviderService>();
        builder.Services.AddDbContext<CourseDbContext>();
        return builder;
    }
}