using LmsApplication.Api.AuthService.Infrastructure;
using LmsApplication.Core.ApplicationServices;
using LmsApplication.Core.Config;
using LmsApplication.Core.Config.Swagger;
using LmsApplication.Core.Data;
using LmsApplication.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpLogging(o => { });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
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

builder.Services.AddCoreApplicationServices();
builder.Services.AddCoreServices();
builder.Services.ConfigureModels(builder.Configuration);

builder.AddAuthDatabase();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        options.TokenValidationParameters.LogValidationExceptions = true;
    }, 
        options =>
    {
    });

builder.Services.AddAuthorization(opt =>
{
    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    opt.AddPolicy("Teacher", new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Teacher", "Admin")
        .Build());

    opt.AddPolicy("Admin", new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build());
});

builder.Services.AddSingleton<IOptionsMonitor<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsProvider>();
builder.Services.AddSingleton<IConfigureOptions<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsInitializer>();

builder.Services.AddSingleton<IOptionsMonitor<JwtBearerOptions>, JwtBearerOptionsProvider>();
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsInitializer>();

// builder.Services.AddSingleton<IOptionsMonitor<MicrosoftGraphOptions>, MicrosoftGraphOptionsProvider>();
// builder.Services.AddSingleton<IConfigureOptions<MicrosoftGraphOptions>, MicrosoftGraphOptionsInitializer>();

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
