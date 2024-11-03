using LmsApplication.Core.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Shared.Database;

public abstract class BaseDbContext : DbContext
{
    public abstract string DatabaseName { get; }
    
    private readonly IConfiguration _config;

    protected BaseDbContext(IConfiguration config)
    {
        _config = config;
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
            entity.CreatedAtUtc = DateTime.UtcNow;
        }
        
        var modified = ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Modified)
            .Select(t => t.Entity)
            .OfType<IAuditable>()
            .ToArray();
        
        foreach (var entity in modified)
        {
            entity.UpdatedAtUtc = DateTime.UtcNow;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _config.GetConnectionString($"{DatabaseName}-db");

        optionsBuilder.UseSqlServer(connectionString, opt => opt.UseAzureSqlDefaults());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}