using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Resources.Yarp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Projects;

var publish = Environment.GetEnvironmentVariable("PUBLISH") == "true";

var builder = DistributedApplication.CreateBuilder(args);

var appconfig = builder.AddConnectionString("AppConfig");

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

// Services
var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService")
    .WithReference(appconfig);

var courseService = builder.AddProject<LmsApplication_Api_CourseService>("courseService")
    .WithReference(appconfig);

var dbInitializer = builder.AddProject<LmsApplication_Core_DbInitializer>("dbInitializer")
    .WithReference(appconfig);

// Create database for each tenant and reference it to the services
List<IResourceBuilder<IResourceWithConnectionString>> dbs = new();

var tenants = builder.Configuration.GetSection(AppTenantsModel.Key).Get<AppTenantsModel>();
foreach (var tenant in tenants!.Tenants)
{
    var db = publish || builder.ExecutionContext.IsPublishMode
        ? builder.AddSqlServer($"db{tenant.Identifier}").AddDatabase("course")
        : builder.AddConnectionString($"db{tenant.Identifier}");
    
    dbs.Add(db);
    
    authService.WithReference(db);
    courseService.WithReference(db);
    dbInitializer.WithReference(db);
}

// Add YARP reverse proxy
builder.AddYarp("yarp")
    .WithEndpoint(8080, scheme: "http")
    .WithReference(authService)
    .WithReference(courseService)
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();
