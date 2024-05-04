using LmsApplication.Resources.Yarp;
using Projects;

var publish = Environment.GetEnvironmentVariable("PUBLISH") == "true";

var builder = DistributedApplication.CreateBuilder(args);

var appconfig = builder.AddConnectionString("AppConfig");

// Services
var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService")
    .WithReference(appconfig);

var dbInitializer = builder.AddProject<LmsApplication_Core_DbInitializer>("dbInitializer")
    .WithReference(appconfig);

// Create database for each tenant and reference it to the services
List<IResourceBuilder<IResourceWithConnectionString>> dbs = new();
foreach (var tenant in builder.Configuration.GetSection("Tenants").GetChildren())
{
    var db = publish || builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureCosmosDB($"db{tenant.Value}").AddDatabase("auth")
        : builder.AddConnectionString($"db{tenant.Value}");
    
    dbs.Add(db);
    
    authService.WithReference(db);
    dbInitializer.WithReference(db);
}

// Add YARP reverse proxy
builder.AddYarp("yarp")
    .WithEndpoint(8080, scheme: "http")
    .WithReference(authService)
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();
