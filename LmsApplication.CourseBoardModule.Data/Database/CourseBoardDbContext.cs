using LmsApplication.Core.Shared.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.CourseBoardModule.Data.Database;

public class CourseBoardDbContext : BaseDbContext
{
    public override string DatabaseName { get; } = "course";

    public CourseBoardDbContext(IConfiguration config) : base(config)
    {
    }
    
    public DbSet<Post> Posts { get; set; } = null!;
    
    public DbSet<Comment> Comments { get; set; } = null!;
    
    public DbSet<PostReaction> PostReactions { get; set; } = null!;
    
    public DbSet<CommentReaction> CommentReactions { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseBoardDbContext).Assembly);
    }
}