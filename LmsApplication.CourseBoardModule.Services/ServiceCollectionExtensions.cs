using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Repositories;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseBoardModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseBoardModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IReactionService, ReactionService>();
        
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<PostCreateModel>, ValidationService<PostCreateModel>>();

        return services;
    }
}