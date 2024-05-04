using LmsApplication.Core.Data.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAuthDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>();
        return builder;
    }
    
    public static void CreateContainers(this HostApplicationBuilder builder)
    {
        var tenants = builder.Configuration.GetSection("Tenants").GetChildren();
        Console.WriteLine("Creating containers...");
        
        foreach (var tenant in tenants)
        {
            Console.WriteLine(tenant.Value);
            var connectionString = builder.Configuration.GetConnectionString($"db{tenant.Value}");
            using (var cosmosClient = new CosmosClient(connectionString))
            {
                var database = cosmosClient.CreateDatabaseIfNotExistsAsync("auth").Result;
                database.Database.CreateContainerIfNotExistsAsync("Users", "/PartitionKey", 400).Wait();
            }
        }
    }
}