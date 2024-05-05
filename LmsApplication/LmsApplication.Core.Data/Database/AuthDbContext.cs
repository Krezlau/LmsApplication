using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Data.Database;

public class AuthDbContext : BaseDbContext
{
    protected override string DatabaseName { get; } = "auth";
    
    public AuthDbContext(IConfiguration config, ITenantProviderService tenantProviderService) : base(config,
        tenantProviderService)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}