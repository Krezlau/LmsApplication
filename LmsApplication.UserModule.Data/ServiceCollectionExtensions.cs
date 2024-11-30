using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("UserDb"));
        });

        services.AddIdentityApiEndpoints<User>(opt =>
            {
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        return services;
    }
}