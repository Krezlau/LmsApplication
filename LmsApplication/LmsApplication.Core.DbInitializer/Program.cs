using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("AppConfig"));
builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddDbContext<CourseDbContext>();

var app = builder.Build();

var tenants = builder.Configuration.GetSection(AppTenantsModel.Key).Get<AppTenantsModel>();
Console.WriteLine("Creating containers...");

foreach (var tenant in tenants!.Tenants)
{
    var tenantService = new MockTenantService(tenants, tenant.Id);
    var db = new CourseDbContext(builder.Configuration, tenantService);
    db.Database.Migrate();
    
    Console.WriteLine($"Container for {tenant.Id} created.");
}

Console.WriteLine("All containers created.");

app.Run();

app.StopAsync().Wait();
app.Dispose();