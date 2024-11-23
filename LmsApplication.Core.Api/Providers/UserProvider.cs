using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Core.Api.Providers;

public class UserProvider : 
    CourseModule.Services.Providers.IUserProvider,
    CourseBoardModule.Services.Providers.IUserProvider,
    ResourceModule.Services.Providers.IUserProvider
{
    private readonly UserManager<User> _userManager;

    public UserProvider(UserManager<User> userManager)
    {
        _userManager = userManager;
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
        var users = await _userManager.Users
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