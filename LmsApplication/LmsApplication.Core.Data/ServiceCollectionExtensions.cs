using LmsApplication.Core.Data.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace LmsApplication.Core.Data;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAuthDatabase(this WebApplicationBuilder builder)
    {
        builder.AddCosmosDbContext<AuthDbContext>("cosmosdb", "Auth");
        return builder;
    }
}