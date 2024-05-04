using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Services.Tenants;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using User = LmsApplication.Core.Data.Entities.User;

namespace LmsApplication.Core.Data.Database;

public class AuthDbContext : DbContext
{
    private const string DatabaseName = "auth";
    
    private readonly IConfiguration _config;
    private readonly ITenantProviderService _tenantProviderService;

    public DbSet<User> Users { get; set; } = null!;
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options,
        IConfiguration config,
        ITenantProviderService tenantProviderService) : base(options)
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
        
        Console.WriteLine(connectionString);
        Console.WriteLine(DatabaseName);
        
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
        modelBuilder.HasDefaultContainer("Users");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}