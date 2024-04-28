using LmsApplication.Core.Data.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAuthDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>();
        return builder;
    }
}