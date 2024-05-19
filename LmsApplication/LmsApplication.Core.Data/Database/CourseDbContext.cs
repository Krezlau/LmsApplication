using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Data.Database;

public class CourseDbContext : BaseDbContext
{
    public override string DatabaseName { get; } = "course";

    public CourseDbContext(
        IConfiguration config,
        ITenantProviderService tenantProviderService) : base(config, tenantProviderService)
    {
    }
    
    public DbSet<Course> Courses { get; set; } = null!;
    
    public DbSet<CourseEdition> CourseEditions { get; set; } = null!;
}