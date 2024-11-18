using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.ResourceModule.Data.Database;

public class ResourceContextFactory : IDesignTimeDbContextFactory<ResourceDbContext>
{
    public ResourceDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().Build();
        return new ResourceDbContext(config);
    }
}