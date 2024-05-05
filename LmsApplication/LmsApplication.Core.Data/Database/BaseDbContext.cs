using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Data.Database;

public abstract class BaseDbContext : DbContext
{
    protected abstract string DatabaseName { get; }
    
    private readonly IConfiguration _config;
    private readonly ITenantProviderService _tenantProviderService;

    protected BaseDbContext(IConfiguration config, ITenantProviderService tenantProviderService)
    {
        _config = config;
        _tenantProviderService = tenantProviderService;
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ChangeTracker.DetectChanges();
        
        var added = ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Added)
            .Select(t => t.Entity)
            .OfType<IAuditable>()
            .ToArray();
        
        foreach (var entity in added)
        {
            entity.Audit.CreatedAt = DateTime.UtcNow;
        }
        
        var modified = ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Modified)
            .Select(t => t.Entity)
            .OfType<IAuditable>()
            .ToArray();
        
        foreach (var entity in modified)
        {
            entity.Audit.UpdatedAt = DateTime.UtcNow;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantId = _tenantProviderService.GetTenantId();
        
        var connectionString = _config.GetConnectionString($"db{tenantId}");
        
        optionsBuilder.UseCosmos(connectionString, DatabaseName, opt =>
        {
            opt.HttpClientFactory(() =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true,
                };

                return new HttpClient(httpMessageHandler);
            });
            opt.ConnectionMode(ConnectionMode.Gateway);
            opt.LimitToEndpoint();
        });
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}