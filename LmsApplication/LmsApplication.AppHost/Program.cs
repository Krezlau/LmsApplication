using LmsApplication.Resources.Yarp;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var appconfig = builder.AddConnectionString("AppConfig");

// Services
var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService")
    .WithReference(appconfig);

// Create database for each tenant and reference it to the services
foreach (var tenant in builder.Configuration.GetSection("Tenants").GetChildren())
{
    var db = builder.AddAzureCosmosDB($"db{tenant.Value}")
        .AddDatabase("Auth")
        .RunAsEmulator(); // local development
    authService = authService.WithReference(db);
}

// Add YARP reverse proxy
builder.AddYarp("yarp")
    .WithEndpoint(8080, scheme: "http")
    .WithReference(authService)
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();