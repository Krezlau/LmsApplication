using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Services;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using LmsApplication.UserModule.Data.Mapping;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.UserModule.Services.Services;

public interface IUserService
{
    Task<UserModel> GetCurrentUserAsync();
    
    Task<UserModel> GetUserByEmailAsync(string userEmail);
    
    Task<List<UserModel>> GetUsersAsync();
    
    Task<List<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId);

    Task<List<UserModel>> SearchUsersByEmailAsync(string query);
    
    Task UpdateUserAsync(string userId, UserUpdateModel model);

    Task UpdateUserRoleAsync(string userId, UpdateUserRoleModel model);
}

public class UserService : IUserService
{
    private readonly UserDbContext _userDbContext;
    private readonly UserManager<User> _userManager;
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IValidationService<UserUpdateModel> _validationService;
    private readonly IUserContext _userContext;

    public UserService(
        UserDbContext userDbContext,
        UserManager<User> userManager,
        IValidationService<UserUpdateModel> validationService,
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext)
    {
        _userDbContext = userDbContext;
        _userManager = userManager;
        _validationService = validationService;
        _courseEditionProvider = courseEditionProvider;
        _userContext = userContext;
    }

    public async Task<UserModel> GetCurrentUserAsync()
    {
        var userId = _userContext.GetUserId();
        var user = await _userDbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");
        
        return user.ToModel();
    }

    public async Task<UserModel> GetUserByEmailAsync(string userEmail)
    {
        var user = await _userDbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == userEmail);
        if (user is null)
            throw new KeyNotFoundException("Couldn't find user with the given email.");

        return user.ToModel();
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await _userDbContext.Users
            .Include(x => x.Roles)
            .ToListAsync();
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task<List<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId)
    {
        var userId = _userContext.GetUserId();
        var isAdmin = _userManager.IsInRoleAsync(new User {Id = userId }, "Admin");
        var isParticipant = _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(courseEditionId, userId);
        
        if (!await isAdmin && !await isParticipant)
        {
            throw new ValidationException("User is not registered to this course edition.");
        }
        
        var userIds = await _courseEditionProvider.GetCourseEditionParticipantsAsync(courseEditionId);
        
        var users = await _userDbContext.Users
            .Include(x => x.Roles)
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync();
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task<List<UserModel>> SearchUsersByEmailAsync(string query)
    {
        var users = await _userDbContext.Users
            .Include(x => x.Roles)
            .Where(x => x.NormalizedEmail.Contains(query.ToUpper()))
            .ToListAsync();
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task UpdateUserAsync(string userId, UserUpdateModel model)
    {
        await _validationService.ValidateAndThrowAsync(model);
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");
        
        user.Name = model.Name;
        user.Surname = model.Surname;
        user.Bio = model.Bio;
        
        await _userManager.UpdateAsync(user);
    }

    public async Task UpdateUserRoleAsync(string userId, UpdateUserRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");

        if (model.Role is UserRole.Student)
        {
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            return;
        }
        var role = await GetRoleByEnumAsync(model.Role);
        
        await _userManager.AddToRoleAsync(user, role.Name!);
    }
    
    private async Task<IdentityRole> GetRoleByEnumAsync(UserRole role)
    {
        var roleName = Enum.GetName(role)?.ToUpper();
        var identityRole = await _userDbContext.Roles.FirstOrDefaultAsync(x => x.Name!.ToUpper() == roleName);
        if (identityRole is null)
        {
            throw new KeyNotFoundException("Couldn't find the role.");
        }

        return identityRole;
    }
}