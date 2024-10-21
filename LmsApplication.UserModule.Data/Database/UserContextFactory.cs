using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.UserModule.Data.Database;

public class UserContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().Build();
        return new UserDbContext(config);
    }
}