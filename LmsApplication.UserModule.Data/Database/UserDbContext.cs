using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.UserModule.Data.Database;

public class UserDbContext : IdentityDbContext<User>
{
    private readonly IConfiguration _config;
    
    public UserDbContext(IConfiguration config) : base()
    {
        _config = config;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _config.GetConnectionString("user-db");

        optionsBuilder.UseSqlServer(connectionString, opt => opt.UseAzureSqlDefaults());
    }
    
}