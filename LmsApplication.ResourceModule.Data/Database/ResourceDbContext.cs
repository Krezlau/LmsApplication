using LmsApplication.Core.Shared.Database;
using LmsApplication.ResourceModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.ResourceModule.Data.Database;

public class ResourceDbContext : BaseDbContext
{
    public override string DatabaseName { get; } = "resource";
    
    public ResourceDbContext(IConfiguration config) : base(config)
    {
    }
    
    public DbSet<ResourceMetadata> ResourceMetadata { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourceDbContext).Assembly);
    }
}