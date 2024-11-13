using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.CourseBoardModule.Data.Database;

public class CourseBoardContextFactory : IDesignTimeDbContextFactory<CourseBoardDbContext>
{
    public CourseBoardDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().Build();
        return new CourseBoardDbContext(config);
    }
}