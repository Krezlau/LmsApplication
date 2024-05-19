using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCoursesDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ITenantProviderService, TenantProviderService>();
        builder.Services.AddDbContext<CourseDbContext>();
        return builder;
    }
    
    public static void CreateContainers(this HostApplicationBuilder builder)
    {
        var tenants = builder.Configuration.GetSection(AppTenantsModel.Key).Get<AppTenantsModel>();
        Console.WriteLine("Creating containers...");
        
        foreach (var tenant in tenants!.Tenants)
        {
            var connectionString = builder.Configuration.GetConnectionString($"db{tenant.Identifier}");
            using (var cosmosClient = new CosmosClient(connectionString))
            {
                var database = cosmosClient.CreateDatabaseIfNotExistsAsync("course", 400).Result;
                Console.WriteLine($"Created database: {database.Database.Id}");
                
                database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties("Courses", "/PartitionKey")).Wait();
                database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties("CourseEditions", "/PartitionKey")).Wait();
            }
        }
    }
}