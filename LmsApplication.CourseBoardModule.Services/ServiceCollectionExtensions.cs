using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.BackgroundServices;
using LmsApplication.CourseBoardModule.Services.Repositories;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LmsApplication.CourseBoardModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseBoardModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IGradesTableRowDefinitionService, GradesTableRowDefinitionService>();
        services.AddScoped<IGradeService, GradeService>();
        
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IGradesTableRowDefinitionRepository, GradesTableRowDefinitionRepository>();
        services.AddScoped<IGradesTableRowValueRepository, GradesTableRowValueRepository>();
        services.AddScoped<IFinalGradeRepository, FinalGradeRepository>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<CreatePostValidationModel>, ValidationService<CreatePostValidationModel>>();
        services.AddScoped<IValidationService<UpdatePostValidationModel>, ValidationService<UpdatePostValidationModel>>();
        services.AddScoped<IValidationService<CreateCommentValidationModel>, ValidationService<CreateCommentValidationModel>>();
        services.AddScoped<IValidationService<UpdateCommentValidationModel>, ValidationService<UpdateCommentValidationModel>>();
        services.AddScoped<IValidationService<UpdateRowDefinitionValidationModel>, ValidationService<UpdateRowDefinitionValidationModel>>();
        services.AddScoped<IValidationService<GradesTableRowDefinitionCreateModel>, ValidationService<GradesTableRowDefinitionCreateModel>>();
        services.AddScoped<IValidationService<UpdateRowValueValidationModel>, ValidationService<UpdateRowValueValidationModel>>();
        services.AddScoped<IValidationService<CreateFinalGradeValidationModel>, ValidationService<CreateFinalGradeValidationModel>>();

        services.AddHostedService<SendPostNotificationsQueuedService>();
        services.AddSingleton<ISendPostNotificationsTaskQueue>(_ =>
        {
            if (!int.TryParse(configuration["BackgroundTaskQueueCapacity"], out var capacity))
                capacity = 100;
            return new SendPostNotificationsTaskQueue(capacity);
        });
        
        return services;
    }
}