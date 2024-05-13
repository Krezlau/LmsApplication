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

var dbInitializer = builder.AddProject<LmsApplication_Core_DbInitializer>("dbInitializer")
    .WithReference(appconfig);

// Create database for each tenant and reference it to the services
List<IResourceBuilder<IResourceWithConnectionString>> dbs = new();

var tenants = builder.Configuration.GetSection(TenantsModel.Key).Get<TenantsModel>();
foreach (var tenant in tenants!.Tenants)
{
    var db = publish || builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureCosmosDB($"db{tenant.Identifier}").AddDatabase("auth")//.RunAsEmulator()
        : builder.AddConnectionString($"db{tenant.Identifier}");
    
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
