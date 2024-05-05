using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using User = LmsApplication.Core.Data.Entities.User;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAuthDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ITenantProviderService, TenantProviderService>();
        builder.Services.AddDbContext<AuthDbContext>();
        return builder;
    }
    
    public static void CreateContainers(this HostApplicationBuilder builder)
    {
        var tenants = builder.Configuration.GetSection(TenantsModel.Key).Get<TenantsModel>();
        Console.WriteLine("Creating containers...");
        
        foreach (var tenant in tenants!.Tenants)
        {
            Console.WriteLine(tenant);
            var connectionString = builder.Configuration.GetConnectionString($"db{tenant}");
            using (var cosmosClient = new CosmosClient(connectionString))
            {
                var database = cosmosClient.CreateDatabaseIfNotExistsAsync("auth").Result;
                database.Database.CreateContainerIfNotExistsAsync("Users", $"/{nameof(User.PartitionKey)}", 400).Wait();
            }
        }
    }
}