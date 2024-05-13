using Finbuckle.MultiTenant;
using LmsApplication.Core.ApplicationServices;
using LmsApplication.Core.Config;
using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Config.Swagger;
using LmsApplication.Core.Data;
using LmsApplication.Core.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddControllers();

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

// builder.Services.AddAuthorization(c =>
// {
//     c.DefaultPolicy = new AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build();
// });
builder.AddAuthDatabase();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = "https://localhost:5000";
        options.ClientId = "jsdflkasj";
        options.ClientSecret = "asjflsdjafl";
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    });

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHeaderStrategy("X-Tenant-Id")
    .WithConfigurationStore(builder.Configuration, TenantsModel.Key)
    .WithPerTenantAuthentication(opt =>
    {
        opt.SkipChallengeIfTenantNotResolved = true;
    });

builder.Services.ConfigurePerTenant<OpenIdConnectOptions, AppTenantInfo>((options, tenantInfo) =>
{
    options.Authority = tenantInfo.OpenIdConnectAuthority ?? string.Empty;
    options.ClientId = tenantInfo.OpenIdConnectClientId ?? string.Empty;
    options.ClientSecret = tenantInfo.OpenIdConnectClientSecret ?? string.Empty;
});

builder.Services.AddLogging();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseMultiTenant();

app.UseCors();

app.Run();
