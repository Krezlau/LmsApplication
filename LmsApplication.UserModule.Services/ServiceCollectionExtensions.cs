using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.UserModule.Data;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Repositories;
using LmsApplication.UserModule.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<UserUpdateModel>, ValidationService<UserUpdateModel>>();

        return services;
    }
    
    public static IServiceCollection AddUserModuleRepositoriesAndData(this IServiceCollection services, IConfiguration config)
    {
        services.AddUserModuleData(config);

        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}