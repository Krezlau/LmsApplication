using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.UserModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<UserUpdateModel>, ValidationService<UserUpdateModel>>();

        return services;
    }
}