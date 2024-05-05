using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using User = LmsApplication.Core.Data.Entities.User;

namespace LmsApplication.Core.Data.Database;

public class AuthDbContext : IdentityDbContext<User>
{
    private string DatabaseName { get; } = "auth";
    
    private readonly IConfiguration _config;
    private readonly ITenantProviderService _tenantProviderService;

    public AuthDbContext(IConfiguration config, ITenantProviderService tenantProviderService)
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

        modelBuilder.Entity<IdentityRole>().ToContainer("Roles");
        modelBuilder.Entity<IdentityRole>().Property(x => x.Id)
            .ToJsonProperty("id");
        modelBuilder.Entity<IdentityRole>().Property(x => x.ConcurrencyStamp)
            .IsETagConcurrency();
        
        modelBuilder.Entity<IdentityUserClaim<string>>().ToContainer("UserClaims");
        
        modelBuilder.Entity<IdentityUserRole<string>>().ToContainer("UserRoles");
        
        modelBuilder.Entity<IdentityUserLogin<string>>().ToContainer("UserLogins");
        
        modelBuilder.Entity<IdentityUserToken<string>>().ToContainer("UserTokens");
        
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToContainer("RoleClaims");
        
        base.OnModelCreating(modelBuilder);
    }
}