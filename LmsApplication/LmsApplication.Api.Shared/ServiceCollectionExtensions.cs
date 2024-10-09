using FluentValidation;
using LmsApplication.Api.Shared.Options;
using LmsApplication.Core.Config;
using LmsApplication.Core.Config.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace LmsApplication.Api.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new() { Title = "LmsApplication.Api.AuthService", Version = "v1" });
            opt.OperationFilter<TenantIdHeaderSwaggerAttribute>();
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
            opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                options.TokenValidationParameters.LogValidationExceptions = true;
            }, 
                options =>
            {
            });

        services.AddAuthorization(opt =>
        {
            opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            opt.AddPolicy(AuthPolicies.TeacherPolicy, new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(UserConstants.AdminRole, UserConstants.TeacherRole)
                .Build());

            opt.AddPolicy(AuthPolicies.AdminPolicy, new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(UserConstants.AdminRole)
                .Build());
        });
        
        
        services.AddSingleton<IOptionsMonitor<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsProvider>();
        services.AddSingleton<IConfigureOptions<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsInitializer>();

        services.AddSingleton<IOptionsMonitor<JwtBearerOptions>, JwtBearerOptionsProvider>();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsInitializer>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
        return services;
    }
}