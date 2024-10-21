using LmsApplication.Core.Api.Middleware;
using LmsApplication.Core.Data.Config;
using LmsApplication.CourseModule.Api;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.UserModule.Api;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpLogging(o => { });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCourseModuleApi(builder.Configuration);
builder.Services.AddUserModuleApi(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(AuthPolicies.AdminPolicy, policy => policy.RequireRole("Admin"));
    opt.AddPolicy(AuthPolicies.TeacherPolicy, policy => policy.RequireRole("Teacher"));
    opt.AddPolicy(AuthPolicies.StudentPolicy, policy => policy.RequireAuthenticatedUser());
    
    opt.DefaultPolicy = opt.GetPolicy(AuthPolicies.StudentPolicy)!;
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "LmsApplication.Api", Version = "v1" });
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


builder.Services.AddLogging();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandler>();

app.MapControllers();
app.MapIdentityApi<User>();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var courseContext = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    courseContext.Database.Migrate();
    var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    userContext.Database.Migrate();
    Console.WriteLine("Migrations applied successfully.");
}

app.Run();

