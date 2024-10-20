using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.CourseModule.Data.Database;

public class CourseContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
{
    public CourseDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().Build();
        return new CourseDbContext(config);
    }
}