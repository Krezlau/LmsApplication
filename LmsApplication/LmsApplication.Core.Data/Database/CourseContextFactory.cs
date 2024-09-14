using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Tenants;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Data.Database;

public class CourseContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
{
    public CourseDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().Build();
        var tenantService = new MockTenantService(new AppTenantsModel() { Tenants = Array.Empty<AppTenantInfo>() }, "course");
        return new CourseDbContext(config, tenantService);
    }
}