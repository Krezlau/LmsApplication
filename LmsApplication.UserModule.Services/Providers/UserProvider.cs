using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;

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
    
    private async Task<UserExchangeModel> MapUserExchangeModelAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
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