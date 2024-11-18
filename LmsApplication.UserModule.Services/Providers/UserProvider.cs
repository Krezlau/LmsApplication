using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.UserModule.Services.Providers;

public class UserProvider : IUserProvider
{
    private readonly UserManager<User> _userManager;

    public UserProvider(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserExchangeModel?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) 
            return null;
        
        return await MapUserExchangeModelAsync(user);
    }

    public async Task<UserExchangeModel?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return null;

        return await MapUserExchangeModelAsync(user);
    }

    public async Task<Dictionary<string, UserExchangeModel>> GetUsersByIdsAsync(List<string> ids)
    {
        var users = await _userManager.Users.Include(x => x.Roles)
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x);
        
        return users.ToDictionary(x => x.Key,
            x => MapUserExchangeModel(x.Value, x.Value.Roles.Select(r => r.Name!).ToList()));
    }

    public async Task<bool> IsUserAdminAsync(string userId)
    {
        return await _userManager.IsInRoleAsync(new User { Id = userId }, "Admin");
    }

    private async Task<UserExchangeModel> MapUserExchangeModelAsync(User user)
    {
        var isAdmin = _userManager.IsInRoleAsync(user, "Admin");
        var isTeacher = _userManager.IsInRoleAsync(user, "Teacher");
        
        var userRole = UserRole.Student;
        if (await isAdmin) 
            userRole = UserRole.Admin;
        else if (await isTeacher)
            userRole = UserRole.Teacher;

        return new UserExchangeModel
        {
            Id = user.Id,
            Email = user.Email!,
            Name = user.Name,
            Surname = user.Surname,
            Role = userRole
        };
    }
    
    private static UserExchangeModel MapUserExchangeModel(User user, List<string> roles)
    {
        var userRole = UserRole.Student;
        if (roles.Contains("Teacher"))
            userRole = UserRole.Teacher;
        else if (roles.Contains("Admin"))
            userRole = UserRole.Admin;

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