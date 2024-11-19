using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.UserModule.Services.Providers;

public class UserProvider : IUserProvider
{
    private readonly UserManager<User> _userManager;
    private readonly UserDbContext _dbContext;

    public UserProvider(UserManager<User> userManager, UserDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<UserExchangeModel?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
            return null;

        return MapUserExchangeModel(user, user.Roles.Select(r => r.Name!).ToList());
    }

    public async Task<Dictionary<string, UserExchangeModel>> GetUsersByIdsAsync(List<string> ids)
    {
        var users = await _dbContext.Users
            .Include(x => x.Roles)
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x);
        
        return users.ToDictionary(x => x.Key,
            x => MapUserExchangeModel(x.Value, x.Value.Roles.Select(r => r.Name!).ToList()));
    }

    private static UserExchangeModel MapUserExchangeModel(User user, List<string> roles)
    {
        var userRole = roles.Select(Enum.Parse<UserRole>).FirstOrDefault();

        return new UserExchangeModel
        {
            Id = user.Id,
            Email = user.Email!,
            Name = user.Name,
            Surname = user.Surname,
            Role = userRole
        };
    }
}