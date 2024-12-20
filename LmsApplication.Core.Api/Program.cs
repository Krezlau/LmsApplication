using LmsApplication.Core.Api.Middleware;
using LmsApplication.Core.Api.Providers;
using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Api;
using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseModule.Api;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.ResourceModule.Api;
using LmsApplication.ResourceModule.Data.Database;
using LmsApplication.UserModule.Api;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpLogging(o => { });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddCourseModuleApi<UserProvider, CourseBoardProvider>(builder.Configuration);
builder.Services.AddCourseBoardModuleApi<UserProvider, CourseEditionProvider>(builder.Configuration);
builder.Services.AddUserModuleApi<CourseEditionProvider>(builder.Configuration);
builder.Services.AddResourceModuleApi<UserProvider, CourseProvider, CourseEditionProvider>(builder.Configuration);

var azureStorage = builder.Configuration.GetConnectionString("StorageConnection");
builder.Services.AddSingleton<IQueueClient<PostNotificationQueueMessage>>(_ =>
    new QueueClient<PostNotificationQueueMessage>(azureStorage, PostNotificationQueueMessage.QueueName));
builder.Services.AddSingleton<IQueueClient<GradeNotificationQueueMessage>>(_ =>
    new QueueClient<GradeNotificationQueueMessage>(azureStorage, GradeNotificationQueueMessage.QueueName));
builder.Services.AddSingleton<IQueueClient<CourseEnrollmentNotificationQueueMessage>>(_ =>
    new QueueClient<CourseEnrollmentNotificationQueueMessage>(azureStorage, CourseEnrollmentNotificationQueueMessage.QueueName));
builder.Services.AddSingleton<IQueueClient<FinalGradeNotificationQueueMessage>>(_ =>
    new QueueClient<FinalGradeNotificationQueueMessage>(azureStorage, FinalGradeNotificationQueueMessage.QueueName));

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
builder.Services.AddApplicationInsightsTelemetry();

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
app.MapCustomIdentityApi<User>();

app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var courseContext = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    courseContext.Database.Migrate();
    var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    userContext.Database.Migrate();
    var courseBoardContext = scope.ServiceProvider.GetRequiredService<CourseBoardDbContext>();
    courseBoardContext.Database.Migrate();
    var resourceContext = scope.ServiceProvider.GetRequiredService<ResourceDbContext>();
    resourceContext.Database.Migrate();
}

app.Run();

