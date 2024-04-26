using LmsApplication.Core.Services.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Data.Database;

public class AuthDbContext : DbContext
{
    private const string DatabaseName = "Auth";
    
    private readonly IConfiguration _config;
    private readonly ITenantProviderService _tenantProviderService;
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options,
        IConfiguration config,
        ITenantProviderService tenantProviderService) : base(options)
    {
        _config = config;
        _tenantProviderService = tenantProviderService;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantId = _tenantProviderService.GetTenantId();
        
        var connectionString = _config.GetConnectionString(tenantId.ToString());
        
        optionsBuilder.UseCosmos(connectionString, DatabaseName);
    }
}