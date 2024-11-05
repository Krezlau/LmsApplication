using FluentValidation;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.Core.Shared.Services;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Providers;
using LmsApplication.UserModule.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleServices(this IServiceCollection services)
    {
        // services
        services.AddScoped<IUserService, UserService>();
        
        // providers
        services.AddScoped<IUserProvider, UserProvider>();
        
        // validators
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<UserUpdateModel>, ValidationService<UserUpdateModel>>();
        

        return services;
    }
}