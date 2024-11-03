using LmsApplication.Core.Shared.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.CourseModule.Data.Database;

public class CourseDbContext : BaseDbContext
{
    public override string DatabaseName { get; } = "course";

    public CourseDbContext(IConfiguration config) : base(config)
    {
    }
    
    public DbSet<Course> Courses { get; set; } = null!;
    
    public DbSet<CourseEdition> CourseEditions { get; set; } = null!;
    
    public DbSet<CourseCategory> CourseCategories { get; set; } = null!;
    
    public DbSet<CourseEditionParticipant> CourseEditionParticipants { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseDbContext).Assembly);
    }
}